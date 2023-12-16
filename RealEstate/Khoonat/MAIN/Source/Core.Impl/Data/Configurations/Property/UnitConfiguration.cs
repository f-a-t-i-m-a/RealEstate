using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class UnitConfiguration : EntityTypeConfiguration<Unit>
	{
		public UnitConfiguration()
		{
			Property(u => u.AdditionalHeatingAndCoolingInformation).HasMaxLength(1000);
			Property(u => u.AdditionalLuxuryFeatures).HasMaxLength(1000);
			Property(u => u.AdditionalSpecialFeatures).HasMaxLength(1000);

			Property(u => u.Area).HasPrecision(10, 3);
			Property(u => u.KitchenArea).HasPrecision(7, 3);
			Property(u => u.BalconyArea).HasPrecision(7, 3);
			Property(u => u.StorageRoomArea).HasPrecision(7, 3);
			Property(u => u.CurrentMonthlyChargeAmount).HasPrecision(17, 2);
			Property(u => u.CeilingHeight).HasPrecision(4, 2);

			HasRequired(r => r.Building)
				.WithMany(b => b.Units)
				.HasForeignKey(r => r.BuildingID)
				.WillCascadeOnDelete(false);
		}
	}
}