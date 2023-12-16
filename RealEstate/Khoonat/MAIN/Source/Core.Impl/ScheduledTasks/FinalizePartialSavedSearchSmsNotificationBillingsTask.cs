using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class FinalizePartialSavedSearchSmsNotificationBillingsTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FinalizePartialSavedSearchSmsNotificationBillingsTask));

        [ComponentPlug]
        public ISavedSearchService SavedSearchService { get; set; }

        public override string Key { get { return ScheduledTaskKeys.FinalizePartialSavedSearchSmsNotificationBillingsTask; } }

        public override int MaxIterationsPerSchedule { get { return 50; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var thereIsMoreWorkToDo = SavedSearchService.FinalizePartialSavedSearchSmsNotificationBillings();
            return ScheduledTaskIterationResult.Success(string.Empty, thereIsMoreWorkToDo: thereIsMoreWorkToDo);
        }
         
    }
}