using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Services;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{

    [Component]
    public class SendNotificationMessageEmailAndSmsTask : ScheduledTaskBase
    {
        [ComponentPlug]
        public INotificationService NotificationService { get; set; }

        public override string Key
        {
            get { return ScheduledTaskKeys.SendNotificationMessageEmailAndSms; }
        }

        public override int MaxIterationsPerSchedule
        {
            get { return 20; } 
        }

        public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
        {
            var hasMoreWork = NotificationService.SendEmailAndSms();
            return ScheduledTaskIterationResult.Success(string.Empty, string.Empty, hasMoreWork);
        }
    }
}
