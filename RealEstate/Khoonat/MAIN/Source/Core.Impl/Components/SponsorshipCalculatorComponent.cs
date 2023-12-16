using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.General;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Ad;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Components
{
    [Component]
    public class SponsorshipCalculatorComponent : ISponsorshipCalculatorComponent
    {
		private static readonly ILog Log = LogManager.GetLogger(typeof(SponsorshipCalculatorComponent));

		public List<TEntity> SelectSponsoredEntities<TEntity>(IEnumerable<TEntity> sponsorships, int numberOfImpressions)
			where TEntity : class, ISponsoredEntityRelated
		{
			if (numberOfImpressions <= 0)
				throw new ArgumentException("numberOfImpressions out of range");

			var selectableItems = sponsorships.Where(si => si.SponsoredEntity.SelectionProbabilityWeight > 0).ToList();
			if (selectableItems.Count <= numberOfImpressions)
				return selectableItems;

			var totalWeight = selectableItems.Sum(si => si.SponsoredEntity.SelectionProbabilityWeight);
			var result = new List<TEntity>();
			var random = RandomProvider.GetThreadRandom();

			Log.DebugFormat("Starting to select impressions from a set of {0} items, total weight: {1}", selectableItems.Count, totalWeight);

			while (result.Count < numberOfImpressions && selectableItems.Count > 0)
			{
				var nextRandom = random.NextDouble() * totalWeight;
				var accumulator = 0d;
				int selectedIndex;

				for (selectedIndex = 0; selectedIndex < selectableItems.Count; selectedIndex++)
				{
					accumulator += selectableItems[selectedIndex].SponsoredEntity.SelectionProbabilityWeight;
					if (accumulator >= nextRandom)
						break;
				}

				if (selectedIndex >= selectableItems.Count)
					break;

				var selectedWeight = selectableItems[selectedIndex].SponsoredEntity.SelectionProbabilityWeight;
				totalWeight -= selectedWeight;

				result.Add(selectableItems[selectedIndex]);
				selectableItems.RemoveAt(selectedIndex);

				Log.DebugFormat("    - Selected index {0} with weight {1} for nextRandom={2}, accumulator={3}.", selectedIndex, selectedWeight, nextRandom, accumulator);
			}

			return result;
		}

		public List<Tuple<TEntity, SponsoredEntityImpression>> CalculateImpressions<TEntity>(
			IEnumerable<TEntity> sponsorships, List<TEntity> selectedSponsorships, SponsorshipTarrif tarrif) 
			where TEntity : class, ISponsoredEntityRelated
        {
			if (sponsorships == null)
				throw new ArgumentNullException("sponsorships");
			if (tarrif == null)
				throw new ArgumentNullException("tarrif");

			var entities = sponsorships.OrderByDescending(e => e.SponsoredEntity.ProjectedMaxPayPerImpression).ToArray();
			var result = new List<Tuple<TEntity, SponsoredEntityImpression>>();

			Log.DebugFormat("Starting to calculate a set of {0} sponsored entities", entities.Length);

			for (int i = 0; i < entities.Length; i++)
            {
				var entity = entities[i];
                var nextEntity = entities.Length <= i + 1 ? null : entities[i + 1];

				if (entity == null)
					continue;

	            if (nextEntity == null)
					Log.DebugFormat("    - Calculating SponsoredEntity ID {0} (next is NULL)", entity.SponsoredEntity.ID);
	            else
					Log.DebugFormat("    - Calculating SponsoredEntity ID {0} (next is {1}, with ProjectedMaxPayPerImpression={2})", entity.SponsoredEntity.ID, nextEntity.SponsoredEntity.ID, nextEntity.SponsoredEntity.ProjectedMaxPayPerImpression);

	            if (selectedSponsorships != null && !selectedSponsorships.Contains(entity))
	            {
					Log.DebugFormat("    -     -> Skipping, the entity is not selected.");
					continue;
	            }

                var impression = new SponsoredEntityImpression();
                impression.GUID = Guid.NewGuid();
                impression.CreationTime = DateTime.Now;
				impression.SponsoredEntityID = entity.SponsoredEntity.ID;
                impression.ContentOwnerUserID = null;
                impression.HttpSessionID = ServiceContext.CurrentSession.Record.ID;
                impression.BillingEntityID = null;

				if (entity.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression)
                {
					impression.BidAmount = nextEntity.IfNotNull(s => s.SponsoredEntity.ProjectedMaxPayPerImpression + tarrif.PerImpressionBidIncrement, tarrif.PerImpressionMinimumBid);
	                impression.BidAmount = Math.Max(impression.BidAmount, tarrif.PerImpressionMinimumBid);
					impression.BidAmount = Math.Min(impression.BidAmount, entity.SponsoredEntity.MaxPayPerImpression);

					Log.DebugFormat("    -     -> Is PerImpression, MaxPayPerImpression={0}, Bidding {1}", entity.SponsoredEntity.MaxPayPerImpression, impression.BidAmount);
                }
				else if (entity.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerClick)
                {
	                impression.BidAmount = nextEntity.IfNotNull(s =>
						Math.Round(s.SponsoredEntity.ProjectedMaxPayPerImpression / entity.SponsoredEntity.EstimatedClicksPerImpression, BillingContants.MoneyPrecision) +
		                tarrif.PerClickBidIncrement,
		                tarrif.PerClickMinimumBid);
	                
					impression.BidAmount = Math.Max(impression.BidAmount, tarrif.PerClickMinimumBid);
					impression.BidAmount = Math.Min(impression.BidAmount, entity.SponsoredEntity.MaxPayPerClick);

					Log.DebugFormat("    -     -> Is PerClick, MaxPayPerClick={0}, EstimatedClicksPerImpression={1}, Bidding {2}",
						entity.SponsoredEntity.MaxPayPerClick, entity.SponsoredEntity.EstimatedClicksPerImpression, impression.BidAmount);
				}
                else
                {
                    impression.BidAmount = 0m;
					Log.DebugFormat("    -     -> Is Subscription-based");
				}

				// Make sure the Bid Amount is rounded correctly
				impression.BidAmount = Math.Round(impression.BidAmount, BillingContants.MoneyPrecision);
				result.Add(new Tuple<TEntity, SponsoredEntityImpression>(entity, impression));
            }

            return result;
        }

		public List<Tuple<TEntity, SponsoredEntityImpression>> SelectAndCalculateImpressions<TEntity>(
			IEnumerable<TEntity> sponsorships, SponsorshipTarrif tarrif, int numberOfImpressions)
			where TEntity : class, ISponsoredEntityRelated
		{
			var selectedSponsorships = SelectSponsoredEntities(sponsorships, numberOfImpressions);
			return CalculateImpressions(sponsorships, selectedSponsorships, tarrif);
		}
    }
}