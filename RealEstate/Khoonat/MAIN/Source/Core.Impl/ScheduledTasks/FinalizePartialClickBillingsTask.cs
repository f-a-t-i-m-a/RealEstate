using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services.Ad;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class FinalizePartialClickBillingsTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FinalizePartialClickBillingsTask));

        [ComponentPlug]
        public ISponsoredEntityService SponsoredEntityService { get; set; }

        public override string Key { get { return ScheduledTaskKeys.FinalizePartialSponsoredEntityClickBillingsTask; } }

        public override int MaxIterationsPerSchedule { get { return 20; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var thereIsMoreWorkToDo = SponsoredEntityService.FinalizePartialClickBillings();
            return ScheduledTaskIterationResult.Success(string.Empty, thereIsMoreWorkToDo: thereIsMoreWorkToDo);
        }
    }
}