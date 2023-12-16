using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.Common.Util.Search;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
	[Component]
	public class OptimizeAllIndexesTask : ScheduledTaskBase
	{
		[ComponentPlug]
		public LuceneIndexManager IndexManager { get; set; }

		public override string Key { get { return ScheduledTaskKeys.OptimizeAllIndexes; } }

		public override int MaxIterationsPerSchedule { get { return 1; } }

		public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
		{
			IndexManager.OptimizeAll();
			IndexManager.CommitAll();

			return ScheduledTaskIterationResult.Success(string.Empty);
		}
	}
}