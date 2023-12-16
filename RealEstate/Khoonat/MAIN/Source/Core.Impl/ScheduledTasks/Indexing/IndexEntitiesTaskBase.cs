using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
	public abstract class IndexEntitiesTaskBase<TEntity> : ScheduledTaskBase where TEntity : class, IIndexedEntity
	{
		[ComponentPlug]
		public IEntityIndex<TEntity> EntityIndex { get; set; }

		[ComponentPlug]
		public IObjectIndex<GlobalSearchIndexItem> GlobalSearchIndex { get; set; }

		[ComponentPlug(false)]
		public IGlobalSearchEntityMapper<TEntity> GlobalSearchEntityMapper { get; set; }

		[ComponentPlug]
		public LuceneIndexManager IndexManager { get; set; }

		[ComponentPlug]
		public IComposer Composer { get; set; }

		public override int MaxIterationsPerSchedule
		{
			get { return 50; }
		}

		public override ScheduledTaskIterationResult IterateInternal(string currentProgress)
		{
			var batchSize = BatchSize;

			var entities = EntitiesQuery
				.Where(pr => !pr.IndexedTime.HasValue)
				.Take(batchSize)
				.AsNoTracking()
				.ToList();

			if (entities.Any())
				EntityIndex.AddOrReplace(entities);

			// Index entity in GlobalSearchIndex, if applicable
			if (GlobalSearchEntityMapper != null)
			{
				var globalSearchItems = entities.Select(GlobalSearchEntityMapper.Map).ToList();
				GlobalSearchIndex.AddOrReplace(globalSearchItems);
			}

			return ScheduledTaskIterationResult.Success(string.Empty, string.Empty, entities.Count >= batchSize);
		}

		public override void OnBatchCompleted()
		{
			// Commit regularly, even if the scheduler doesn't add any documents, to update health status.
			IndexManager.Commit(EntityIndex.IndexID);

			if (GlobalSearchEntityMapper != null)
				IndexManager.Commit(GlobalSearchIndex.IndexID);
		}

		protected abstract IQueryable<TEntity> EntitiesQuery { get; }

		protected virtual int BatchSize
		{
			get { return 100; }
		}
	}
}