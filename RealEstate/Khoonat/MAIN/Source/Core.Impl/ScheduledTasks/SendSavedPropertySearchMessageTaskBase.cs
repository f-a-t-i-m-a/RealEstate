using System;
using System.Globalization;
using System.Linq;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
	public abstract class SendSavedPropertySearchMessageTaskBase : ScheduledTaskBase
	{
	    protected abstract int SendMessagesToTargets(long listingId);

        public override int MaxIterationsPerSchedule { get { return 20; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
		{
			var currentProgressDt = ParseProgress(currentProgress);
			var nextListing = FindNextPropertyListing(currentProgressDt);

			if (nextListing == null)
				return ScheduledTaskIterationResult.Success(currentProgress ?? currentProgressDt.Ticks.ToString(CultureInfo.InvariantCulture));

			var savedSearchCount = SendMessagesToTargets(nextListing.ID);
			nextListing.NumberOfSearches += savedSearchCount;

            CommonStaticLogs.ScheduledTask.InfoFormat(
                "Listing ID {0} processed by {1}, matched {2} saved searches.", nextListing.ID, Key, savedSearchCount);

			return ScheduledTaskIterationResult.Success(
				(nextListing.ModificationDate.Ticks + 1L).ToString(CultureInfo.InvariantCulture),
				"Listing ID " + nextListing.ID + " processed, " + savedSearchCount + " saved searches matched.",
                thereIsMoreWorkToDo: true);
		}

		private DateTime ParseProgress(string currentProgress)
		{
			// If the current progress is null, this is the first time the schedule is running.
			// Start from right now.

			if (string.IsNullOrWhiteSpace(currentProgress))
				return DateTime.Now;

			long currentProgressTicks;
			if (!long.TryParse(currentProgress, out currentProgressTicks))
				throw new ArgumentException("Invalid progress string. A valid Ticks number is expected.");

			return new DateTime(currentProgressTicks);
		}

		private PropertyListing FindNextPropertyListing(DateTime currentProgressDt)
		{
			// Matching properties should:
			//   - is public (is published, and been reviewed and accepted by operator)
			//   - not been changed very recently (otherwise it results in multiple notifications while users are editing)

			var halfHourAgo = DateTime.Now.AddMinutes(-30);

			// To deliver messages instantly in development and test environments
			if (ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Development || ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Test)
				halfHourAgo = DateTime.Now;

			if (currentProgressDt > halfHourAgo)
				return null;

			// The notification mechanism will load the full details separately, so we don't need to
			// include any of the attached properties here. Just the listing info will suffice.

			// Using DbSet to be able to increase "number of searches" on it, depending on the 
			// matched saved searches.

			return DbManager.Db.PropertyListingsDbSet
				.Where(PropertyListingExtensions.GetPublicListingExpression())
				.Where(pl => pl.ModificationDate >= currentProgressDt && pl.ModificationDate < halfHourAgo)
				.OrderBy(pl => pl.ModificationDate)
				.FirstOrDefault();
		}
	}
}