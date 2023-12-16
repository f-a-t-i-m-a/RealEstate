using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Directory
{
    public class AgencyConfiguration : EntityTypeConfiguration<Agency>
    {
        public AgencyConfiguration()
        {
	        Ignore(a => a.Content);
	        Property(a => a.ContentString).HasMaxLength(null);

            HasMany(a => a.MemberUsers)
                .WithOptional(u => u.Agency)
                .HasForeignKey(u => u.AgencyID)
                .WillCascadeOnDelete(false);
        }
    }
}