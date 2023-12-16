using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class PropertyListingContactInfoConfiguration : EntityTypeConfiguration<PropertyListingContactInfo>
	{
		public PropertyListingContactInfoConfiguration()
		{
			Property(ci => ci.AgencyName).HasMaxLength(80);
			Property(ci => ci.AgencyAddress).HasMaxLength(200);
			Property(ci => ci.ContactName).HasMaxLength(80);
			Property(ci => ci.ContactPhone1).HasMaxLength(25);
			Property(ci => ci.ContactPhone2).HasMaxLength(25);
			Property(ci => ci.ContactEmail).HasMaxLength(100);
			Property(ci => ci.OwnerName).HasMaxLength(80);
			Property(ci => ci.OwnerPhone1).HasMaxLength(25);
			Property(ci => ci.OwnerPhone2).HasMaxLength(25);
			Property(ci => ci.OwnerEmail).HasMaxLength(100);
		}
	}
}