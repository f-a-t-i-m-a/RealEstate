using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services.Ad;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    [Component]
    public class RecalculateSponsoredEntitiesTask : ScheduledTaskBase
    {
        [ComponentPlug]
        public ISponsoredEntityCalculationService SponsoredEntityCalculationService { get; set; }

        public override string Key
        {
            get { return ScheduledTaskKeys.RecalculateSponsoredEntitiesTask; }
        }

        public override int MaxIterationsPerSchedule
        {
            get { return 40; }
        }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
			var thereIsMoreWorkToDo = SponsoredEntityCalculationService.RecalculateNextBatch();
			return ScheduledTaskIterationResult.Success(string.Empty, thereIsMoreWorkToDo: thereIsMoreWorkToDo);
        }
    }
}