using Compositional.Composer;

namespace JahanJooy.Common.Util.ScheduledTasks
{
	[Contract]
	public interface IScheduledTask
	{
		string Key { get; }
	    int MaxIterationsPerSchedule { get; }

	    void OnBatchStarting();
		void OnBatchCompleted();

	    ScheduledTaskIterationResult Iterate(string currentProgress);
	}
}