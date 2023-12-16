using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services.Ad;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class BillSponsoredEntityImpressionsTask : ScheduledTaskBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (BillSponsoredEntityImpressionsTask));

        #region Injected Dependencies

        [ComponentPlug]
        public ISponsoredEntityService SponsoredEntityService { get; set; }

        #endregion

        public override string Key
        {
            get { return ScheduledTaskKeys.BillSponsoredEntityImpressionsTask; }
        }

        public override int MaxIterationsPerSchedule
        {
            get { return 20; }
        }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var thereIsMoreWorkToDo = SponsoredEntityService.BillImpressions();
            return ScheduledTaskIterationResult.Success(string.Empty, thereIsMoreWorkToDo: thereIsMoreWorkToDo);
        }
    }
}