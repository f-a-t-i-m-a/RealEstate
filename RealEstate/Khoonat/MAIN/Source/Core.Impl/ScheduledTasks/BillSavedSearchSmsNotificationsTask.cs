using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class BillSavedSearchSmsNotificationsTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BillSavedSearchSmsNotificationsTask));

        [ComponentPlug]
        public ISavedSearchService SavedSearchService { get; set; }

        public override string Key { get { return ScheduledTaskKeys.BillSavedSearchSmsNotificationsTask; } }

        public override int MaxIterationsPerSchedule { get { return 20; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var thereIsMoreWorkToDo = SavedSearchService.BillSavedSearchSmsNotifications();
            return ScheduledTaskIterationResult.Success(string.Empty, thereIsMoreWorkToDo: thereIsMoreWorkToDo);
        }
    }
}