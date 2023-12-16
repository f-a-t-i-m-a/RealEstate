using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
    [Component]
    public class IndexAgenciesTask : IndexEntitiesTaskBase<Agency>
    {
        public override string Key
        {
            get { return ScheduledTaskKeys.IndexAgencies; }
        }

        protected override IQueryable<Agency> EntitiesQuery
        {
            get { return DbManager.Db.AgenciesDbSet.Include(a => a.AgencyBranches).OrderByDescending(a => a.ID); }
        }
    }
}