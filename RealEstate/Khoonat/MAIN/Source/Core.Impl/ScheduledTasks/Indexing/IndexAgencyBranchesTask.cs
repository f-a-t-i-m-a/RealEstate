using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks.Indexing
{
    [Component]
    public class IndexAgencyBranchesTask : IndexEntitiesTaskBase<AgencyBranch>
    {
        public override string Key
        {
            get { return ScheduledTaskKeys.IndexAgencyBranches; }
        }

        protected override IQueryable<AgencyBranch> EntitiesQuery
        {
            get { return DbManager.Db.AgencyBranchesDbSet.Include(ab => ab.Agency).OrderByDescending(ab => ab.ID); }
        }
    }
}