using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Directory
{
    public class AgencyBranchConfiguration : EntityTypeConfiguration<AgencyBranch>
    {
        public AgencyBranchConfiguration()
        {
			Ignore(a => a.Content);
			Property(a => a.ContentString).HasMaxLength(null);

			HasRequired(ab => ab.Agency)
                .WithMany(a => a.AgencyBranches)
                .HasForeignKey(ab => ab.AgencyID)
                .WillCascadeOnDelete(true);
        }
    }
}