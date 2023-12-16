using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
	[Component]
	public class CheckSmsMessageDeliveryTask : ScheduledTaskBase
	{
        [ComponentPlug]
        public ISmsMessageService SmsMessageService { get; set; }
        
        public override string Key { get { return ScheduledTaskKeys.CheckSmsMessageDeliveryTask; } }

        public override int MaxIterationsPerSchedule { get { return 20; } }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
		{
            var hasMoreWork = SmsMessageService.RunDeliveryCheckingBatch();
            return ScheduledTaskIterationResult.Success(string.Empty, string.Empty, hasMoreWork);
        }
	}
}