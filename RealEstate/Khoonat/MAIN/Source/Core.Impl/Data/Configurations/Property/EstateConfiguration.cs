using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class EstateConfiguration : EntityTypeConfiguration<Estate>
	{
		public EstateConfiguration()
		{
			Property(e => e.Address).HasMaxLength(150);
			Property(e => e.AdditionalAddressInfo).HasMaxLength(1000);

			Property(e => e.Area).HasPrecision(10, 3);
			Property(e => e.PassageEdgeLength).HasPrecision(6, 2);
			Property(e => e.PassageWidth).HasPrecision(5, 2);

            HasOptional(e => e.Vicinity)
                .WithMany()
                .HasForeignKey(e => e.VicinityID)
                .WillCascadeOnDelete(false);
		}
	}
}