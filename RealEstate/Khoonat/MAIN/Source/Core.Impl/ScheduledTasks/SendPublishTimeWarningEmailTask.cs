using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Services;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
	[Component]
    public class SendPublishTimeWarningEmailTask : ScheduledTaskBase
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(SendPublishTimeWarningEmailTask));

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        public override string Key { get { return ScheduledTaskKeys.SendPublishTimeWarningEmailTask; } }

        public override int MaxIterationsPerSchedule { get { return 1; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            DateTime fourDaysLater = DateTime.Now.AddDays(4);

            var currentProgressDt = ParseProgress(currentProgress);
            var listingIds = FindPropertyListingsExpiringBetween(currentProgressDt, fourDaysLater);

            if (listingIds == null)
                return ScheduledTaskIterationResult.Success(currentProgress ?? currentProgressDt.Ticks.ToString(CultureInfo.InvariantCulture));

            foreach (var listingId in listingIds)
            {
                try
                {
                    EmailNotificationService.NotifyPublishAboutToExpire(listingId);
                }
                catch (Exception e)
                {
                    // Catch exceptions in email transmission to prevent one failure aborting a whole batch
                    Log.Error("Caught an exception while sending publish expiration warning email for property listing " + listingId, e);
                }
            }

            return ScheduledTaskIterationResult.Success((fourDaysLater.Ticks + 1L).ToString(CultureInfo.InvariantCulture), string.Empty);
        }

        private DateTime ParseProgress(string currentProgress)
		{
			// If the current progress is null, this is the first time the schedule is running.
			// get the list from tomorrow to 4 days later

            if (string.IsNullOrWhiteSpace(currentProgress))
                return DateTime.Now.AddDays(1);

            long currentProgressTicks;
			if (!long.TryParse(currentProgress, out currentProgressTicks))
				throw new ArgumentException("Invalid progress string. A valid Ticks number is expected.");

			return new DateTime(currentProgressTicks);
		}

        private IEnumerable<long> FindPropertyListingsExpiringBetween(DateTime currentProgressDt, DateTime fourDaysLater)
		{
            return DbManager.Db.PropertyListingsDbSet
                .Where(PropertyListingExtensions.GetPublicListingExpression())
                .Where(pl => pl.PublishEndDate >= currentProgressDt && pl.PublishEndDate < fourDaysLater)
                .Select(pl => pl.ID)
                .ToList();

		}
	}
}