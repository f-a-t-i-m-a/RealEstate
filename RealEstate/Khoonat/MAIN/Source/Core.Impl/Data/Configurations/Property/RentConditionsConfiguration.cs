using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class RentConditionsConfiguration : EntityTypeConfiguration<RentConditions>
	{
		public RentConditionsConfiguration()
		{
			Property(rc => rc.Mortgage).HasPrecision(17, 2);
			Property(rc => rc.Rent).HasPrecision(17, 2);
			Property(rc => rc.MinimumMortgage).HasPrecision(17, 2);
			Property(rc => rc.MinimumRent).HasPrecision(17, 2);
		}
	}
}