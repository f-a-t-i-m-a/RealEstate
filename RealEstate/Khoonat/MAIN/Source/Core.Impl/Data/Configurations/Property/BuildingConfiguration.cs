using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class BuildingConfiguration : EntityTypeConfiguration<Building>
	{
		public BuildingConfiguration()
		{
			Property(b => b.OtherFeatures).HasMaxLength(1000);
			Property(b => b.OtherWelfareFeatures).HasMaxLength(1000);
			Property(b => b.OtherRecreationFeatures).HasMaxLength(1000);
			Property(b => b.OtherSafetyFeatures).HasMaxLength(1000);

			HasRequired(b => b.Estate)
				.WithMany(e => e.Buildings)
				.HasForeignKey(b => b.EstateID)
				.WillCascadeOnDelete(false);
		}
	}
}