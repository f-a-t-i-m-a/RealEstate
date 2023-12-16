using Compositional.Composer;

namespace JahanJooy.Common.Util.ScheduledTasks
{
	[Contract]
	public interface IScheduledTaskManager
	{
		void IterateTask(string key, bool isManuallyTriggered = false);
		void ChangeTaskStartup(string key, RecurringTaskStartupMode startupMode);
	}
}