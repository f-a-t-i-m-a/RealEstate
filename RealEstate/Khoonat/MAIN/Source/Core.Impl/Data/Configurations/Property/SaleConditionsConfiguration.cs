using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class SaleConditionsConfiguration : EntityTypeConfiguration<SaleConditions>
	{
		public SaleConditionsConfiguration()
		{
			Property(sc => sc.Price).HasPrecision(17, 2);
			Property(sc => sc.PricePerEstateArea).HasPrecision(17, 2);
			Property(sc => sc.PricePerUnitArea).HasPrecision(17, 2);
			Property(sc => sc.MinimumMonthlyPaymentForDebt).HasPrecision(17, 2);
			Property(sc => sc.TransferableLoanAmount).HasPrecision(17, 2);
		}
	}
}