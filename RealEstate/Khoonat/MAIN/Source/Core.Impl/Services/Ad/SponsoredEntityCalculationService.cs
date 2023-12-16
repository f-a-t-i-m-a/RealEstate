using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Messages;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Services.Ad
{
	[Component]
	public class SponsoredEntityCalculationService : ISponsoredEntityCalculationService
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(SponsoredEntityCalculationService));

		private const int BatchSize = 20;

		private static readonly TimeSpan MaxRecalcTimeSpan = TimeSpan.FromDays(10);
		private static readonly TimeSpan MinRecalcTimeSpan = TimeSpan.FromMinutes(10);
		private static readonly TimeSpan DurationToKeepRecalcBlockedEntity = TimeSpan.FromDays(40);

		#region Component plugs

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IUserBillingBalanceCache BalanceCache { get; set; }

		[ComponentPlug]
		public IUserBalanceService UserBalanceService { get; set; }

		#endregion

		public bool RecalculateNextBatch()
		{
			var sponsoredEntities = DbManager.Db.SponsoredEntitiesDbSet
				.Where(s => s.NextRecalcDue <= DateTime.Now && s.Enabled && !s.DeleteTime.HasValue &&
							(s.ExpirationTime > DateTime.Now || !s.ExpirationTime.HasValue))
				.Take(BatchSize)
				.ToList();

			Log.InfoFormat("Recalculating {0} sponsored entities: {1}", sponsoredEntities.Count, string.Join(",", sponsoredEntities.Select(se => se.ID)));

			foreach (var sponsoredEntity in sponsoredEntities)
			{
				Recalculate(sponsoredEntity);
			}

			return sponsoredEntities.Count >= BatchSize;
		}

		public void RecalculateEntity(long sponsoredEntityId)
		{
			Log.InfoFormat("Recalculating single sponsored entity: {0}", sponsoredEntityId);

			var sponsoredEntity = DbManager.Db.SponsoredEntitiesDbSet
				.SingleOrDefault(se => se.ID == sponsoredEntityId);

			Recalculate(sponsoredEntity);
		}

		#region Private implementations

		private void Recalculate(SponsoredEntity entity)
		{
			Log.DebugFormat("Starting recalculation for sponsored entity {0}", entity.ID);

			var data = LoadCalculationData(entity);

			RecalculateBlocked(entity, data);
			RecalculateNextRecalcDue(entity, data);
			EnforceNextRecalcDueMinMax(entity);

			UserBalanceService.OnUserCostProcessed(data.UserBalance.Value, entity.ID, NotificationSourceEntityType.SponsoredEntity);
			if (entity.BlockedForLowCredit)
			{
				// If the sponsored entity is blocked, it won't be shown unless another recalc takes place.
				// So there's no need to recalc the rest of the properties.
				return;
			}

			RecalculateClicksPerImpression(entity, data);
			entity.EstimatedClicksPerImpression = Math.Max(entity.EstimatedClicksPerImpression, AdConstants.MinimumClicksPerImpression);
			entity.EstimatedClicksPerImpression = Math.Min(entity.EstimatedClicksPerImpression, AdConstants.MaximumClicksPerImpression);
			Log.DebugFormat("[{0}] Final ClicksPerImpression = {1}", entity.ID, entity.EstimatedClicksPerImpression);

			RecalculateProjectedPayPerImpression(entity);
			RecalculateSelectionProbabilityWeight(entity, data);
		}

		private CalculationData LoadCalculationData(SponsoredEntity entity)
		{
			var oneDayAgo = DateTime.Now.AddDays(-1);
			var result = new CalculationData
			{
				SponsoredEntityID = entity.ID,
				UserBalance = new Lazy<UserBillingBalance>(() => BalanceCache[entity.BilledUserID]),
				NumberOfImpressions = new Lazy<long>(() =>
					DbManager.Db.SponsoredEntityImpressions.Count(i => i.SponsoredEntityID == entity.ID)),
				NumberOfClicks = new Lazy<long>(() =>
					DbManager.Db.SponsoredEntityClicks.Count(c => c.SponsoredEntityID == entity.ID)),
				Last24HImpressions = new Lazy<double>(
					() => DbManager.Db.SponsoredEntityImpressions.Count(i => i.SponsoredEntityID == entity.ID && i.CreationTime > oneDayAgo)),
				Last24HClicks = new Lazy<double>(
					() => DbManager.Db.SponsoredEntityClicks.Count(i => i.SponsoredEntityID == entity.ID && i.CreationTime > oneDayAgo)),
				FirstImpressionTimeInLast24H = new Lazy<DateTime?>(
					() => DbManager.Db.SponsoredEntityImpressions.Where(i => i.SponsoredEntityID == entity.ID && i.CreationTime > oneDayAgo).Select(i => i.CreationTime).Cast<DateTime?>().Min()),
				FirstClickTimeInLast24H = new Lazy<DateTime?>(
					() => DbManager.Db.SponsoredEntityClicks.Where(i => i.SponsoredEntityID == entity.ID && i.CreationTime > oneDayAgo).Select(i => i.CreationTime).Cast<DateTime?>().Min())
			};

			result.NormalizedImpressionsPer24H = new Lazy<double>(
				() =>
				{
					var impressions = Math.Max(5, result.Last24HImpressions.Value);

					if (result.FirstImpressionTimeInLast24H.Value.HasValue)
					{
						// Normalize according to the first impression in the last 24h
						var timeSpan = DateTime.Now - result.FirstImpressionTimeInLast24H.Value.Value;
						impressions *= (TimeSpan.FromDays(1).TotalSeconds / timeSpan.TotalSeconds);
					}

					return impressions;
				});

			result.NormalizedClicksPer24H = new Lazy<double>(
				() =>
				{
					var clicks = Math.Max(5, result.Last24HClicks.Value);

					if (result.FirstClickTimeInLast24H.Value.HasValue)
					{
						// Normalize according to the first click in the last 24h
						var timeSpan = DateTime.Now - result.FirstClickTimeInLast24H.Value.Value;
						clicks *= (TimeSpan.FromDays(1).TotalSeconds / timeSpan.TotalSeconds);
					}

					return clicks;
				});

			if (Log.IsDebugEnabled)
				result.LogCalculationData();

			return result;
		}

		private void RecalculateProjectedPayPerImpression(SponsoredEntity entity)
		{
			if (entity.BillingMethod == SponsoredEntityBillingMethod.PerImpression)
			{
				entity.ProjectedMaxPayPerImpression = entity.MaxPayPerImpression;
			}
			else
			{
				entity.ProjectedMaxPayPerImpression = entity.MaxPayPerClick * entity.EstimatedClicksPerImpression;
			}

			Log.DebugFormat("[{0}] Final ProjectedMaxPayPerImpression = {1}", entity.ID, entity.ProjectedMaxPayPerImpression);
		}

		private void RecalculateSelectionProbabilityWeight(SponsoredEntity entity, CalculationData data)
		{
			var weight = decimal.ToDouble(entity.ProjectedMaxPayPerImpression);
			double boost = 1;

			if (data.UserBalance.Value.TotalBalance > 0)
			{
				boost *= Math.Min(3d, Math.Max(0.5d, decimal.ToDouble(data.UserBalance.Value.CashBalance)/decimal.ToDouble(data.UserBalance.Value.TotalBalance)));
			}

			entity.SelectionProbabilityWeight = weight*boost;
		}

		private void RecalculateClicksPerImpression(SponsoredEntity entity, CalculationData data)
		{
			// If the entity is newly created, use the default value
			if (entity.CreationTime.AddDays(5) > DateTime.Now)
			{
				Log.DebugFormat("[{0}] Is newly created, using default ClicksPerImpression", entity.ID);

				entity.EstimatedClicksPerImpression = AdConstants.DefaultClicksPerImpression;
				return;
			}

			// If the entity has not received enough impressions, ignore the historical figures.
			if (data.NumberOfImpressions.Value < 20)
			{
				Log.DebugFormat("[{0}] Has not received enough impressions, using default ClicksPerImpression", entity.ID);

				entity.EstimatedClicksPerImpression = AdConstants.DefaultClicksPerImpression;
				return;
			}

			// If the entity has received many impressions, calculate the estimate solely based on the historical figures
			var historicalClicksPerImpression = ((decimal)data.NumberOfClicks.Value) / data.NumberOfImpressions.Value;
			if (data.NumberOfImpressions.Value >= 1000)
			{
				Log.DebugFormat("[{0}] Has many impressions, only using historical data", entity.ID);
				
				entity.EstimatedClicksPerImpression = historicalClicksPerImpression;
				return;
			}

			// Calculate a weighted average of the Default clicks per impression, and the historical figures.
			Log.DebugFormat("[{0}] Using weighted average of historical data and default value for ClicksPerImpression", entity.ID);

			entity.EstimatedClicksPerImpression = (data.NumberOfImpressions.Value * historicalClicksPerImpression + (1000 - data.NumberOfImpressions.Value) * AdConstants.DefaultClicksPerImpression) / 1000;
		}

		private void RecalculateBlocked(SponsoredEntity entity, CalculationData data)
		{
			var balance = data.UserBalance.Value;

			if (!balance.CanSpend)
			{
				Log.DebugFormat("[{0}] Blocking for low balance. Previously blocked: {1}", entity.ID, entity.BlockedForLowCredit);

				entity.BlockedForLowCredit = true;
			}
			else
			{
				if (entity.BlockedForLowCredit)
				{
					Log.DebugFormat("[{0}] UN-Blocking", entity.ID);
					// TODO: Keep a "LastUnblock" time, and use it in calculations
				}
				else
				{
					Log.DebugFormat("[{0}] Is not blocked", entity.ID);
				}

				entity.BlockedForLowCredit = false;
			}
		}

		private void RecalculateNextRecalcDue(SponsoredEntity entity, CalculationData data)
		{
			if (entity.BlockedForLowCredit)
			{
				// Normally, when the user makes a payment, the recalc due should be re-set.
				// So, set the next recalc due to sometime far in the future to avoid frequent recalcs, but
				// allow covering development mistakes where the recalc due is not reset when the user
				// balance increases.

				// If there's a long time since the last Impression of the sponsored entity, set the next
				// due to NULL so that it is not re-calculated forever.

				var lastActivity = entity.LastImpressionTime ?? entity.CreationTime;
				if ((DateTime.Now - lastActivity).Duration() > DurationToKeepRecalcBlockedEntity)
				{
					Log.DebugFormat("[{0}] Has been blocked for too long, NextRecalc will be null.", entity.ID);
					entity.NextRecalcDue = null;
				}
				else
				{
					Log.DebugFormat("[{0}] Is blocked, NextRecalc will be after max duration.", entity.ID);
					entity.NextRecalcDue = DateTime.Now + MaxRecalcTimeSpan;
				}

				return;
			}

			if (entity.BillingMethod != SponsoredEntityBillingMethod.PerImpression && entity.BillingMethod != SponsoredEntityBillingMethod.PerClick)
			{
				// Subscription based billing,
				// We don't know how to recalc yet.

				Log.DebugFormat("[{0}] Is not of supported type for calculating NextRecalc.", entity.ID);
				entity.NextRecalcDue = DateTime.Now.Date.AddDays(1);
				return;
			}

			double minDaysToExhaustion;

			if (entity.BillingMethod == SponsoredEntityBillingMethod.PerImpression)
			{
				var minImpressionsToExhaustion = (double)(data.UserBalance.Value.TotalBalance / entity.MaxPayPerImpression);
				minDaysToExhaustion = minImpressionsToExhaustion / data.NormalizedImpressionsPer24H.Value;

				Log.DebugFormat("[{0}] Is PayPerImpression, user balance divided to MaxPayPerImpression = {1}, results in minDaysToExhaustion = {2}", entity.ID, entity.MaxPayPerImpression, minDaysToExhaustion);
			}
			else // if (entity.BillingMethod == SponsoredEntityBillingMethod.PerClick)
			{
				var minClicksToExhaustion = (double)(data.UserBalance.Value.TotalBalance / entity.MaxPayPerClick);
				minDaysToExhaustion = minClicksToExhaustion / data.NormalizedClicksPer24H.Value;

				Log.DebugFormat("[{0}] Is PayPerClick, user balance divided to MaxPayPerClick = {1}, results in minDaysToExhaustion = {2}", entity.ID, entity.MaxPayPerClick, minDaysToExhaustion);
			}

			var daysToNextRecalc = minDaysToExhaustion * AdConstants.RecalcDueCoefficient;
			Log.DebugFormat("[{0}] Will be recalced after {1} days", entity.ID, daysToNextRecalc);

			entity.NextRecalcDue = DateTime.Now.AddDays(daysToNextRecalc);
		}

		private void EnforceNextRecalcDueMinMax(SponsoredEntity entity)
		{
			if (!entity.NextRecalcDue.HasValue)
			{
				Log.DebugFormat("[{0}] Final NextCalc = NULL", entity.ID);
				return;
			}

			// First, adjust the next due according to creation time of the sponsored entity

			// TODO: Use last-unblock-time for this purpose to cover all scenarios like edit, block and payment, etc.
			var durationSinceCreation = (DateTime.Now - entity.CreationTime).Duration();
			var maxRecalcDueFromCreationTime = DateTime.Now + new TimeSpan(durationSinceCreation.Ticks / 3);

			if (entity.NextRecalcDue.Value > maxRecalcDueFromCreationTime)
				entity.NextRecalcDue = maxRecalcDueFromCreationTime;

			// At last, enforce global constraints on the recalc.

			var maxRecalcDue = DateTime.Now + MaxRecalcTimeSpan;
			var minRecalcDue = DateTime.Now + MinRecalcTimeSpan;

			if (entity.NextRecalcDue.Value > maxRecalcDue)
				entity.NextRecalcDue = maxRecalcDue;

			if (entity.NextRecalcDue < minRecalcDue)
				entity.NextRecalcDue = minRecalcDue;

			Log.DebugFormat("[{0}] Final NextRecalc = {1}", entity.ID, entity.NextRecalcDue);
		}

		private class CalculationData
		{
			public long SponsoredEntityID { get; set; }

			public Lazy<UserBillingBalance> UserBalance { get; set; }

			public Lazy<long> NumberOfImpressions { get; set; }
			public Lazy<long> NumberOfClicks { get; set; }

			public Lazy<double> Last24HImpressions { get; set; }
			public Lazy<double> Last24HClicks { get; set; }

			public Lazy<DateTime?> FirstImpressionTimeInLast24H { get; set; }
			public Lazy<DateTime?> FirstClickTimeInLast24H { get; set; }

			public Lazy<double> NormalizedImpressionsPer24H { get; set; }
			public Lazy<double> NormalizedClicksPer24H { get; set; }

			public void LogCalculationData()
			{
				Log.DebugFormat("Logging re-calculation data for SponsoredEntity ID {0}:", SponsoredEntityID);
				Log.DebugFormat("    - Balance for UserID {0} = {1} cash + {2} bonus, {3} total.",
					UserBalance.Value.UserID, UserBalance.Value.CashBalance, UserBalance.Value.BonusBalance, UserBalance.Value.TotalBalance);
				Log.DebugFormat("    - Impressions = {0}, {1} of which in last 24h -> Normalized to {2}", 
					NumberOfImpressions.Value, Last24HImpressions.Value, NormalizedImpressionsPer24H.Value);
				Log.DebugFormat("    - Clicks      = {0}, {1} of which in last 24h -> Normalized to {2}", 
					NumberOfClicks.Value, Last24HClicks.Value, NormalizedClicksPer24H.Value);
			}
		}

		#endregion
	}
}