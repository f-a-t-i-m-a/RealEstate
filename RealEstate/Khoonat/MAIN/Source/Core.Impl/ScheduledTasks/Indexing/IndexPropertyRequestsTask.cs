using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
	[Component]
	public class IndexPropertyRequestsTask : IndexEntitiesTaskBase<PropertyRequest>
	{
		public override string Key
		{
			get { return ScheduledTaskKeys.IndexPropertyRequests; }
		}

		protected override IQueryable<PropertyRequest> EntitiesQuery
		{
			get { return DbManager.Db.PropertyRequestsDbSet.OrderByDescending(pr => pr.ID); }
		}
	}
}