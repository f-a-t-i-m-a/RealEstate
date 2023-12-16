using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.DomainUtil;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
	[Component]
	public class IndexPropertyListingsTask : IndexEntitiesTaskBase<PropertyListing>
	{
		public override string Key
		{
			get { return ScheduledTaskKeys.IndexPropertyListings; }
		}

		protected override IQueryable<PropertyListing> EntitiesQuery
		{
			get { return DbManager.Db.PropertyListings.IncludeInfoProperties().OrderByDescending(pr => pr.ID); }
		}
	}
}