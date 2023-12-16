using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class PropertyListingPhotoConfiguration : EntityTypeConfiguration<PropertyListingPhoto>
	{
		public PropertyListingPhotoConfiguration()
		{
			Property(plp => plp.Title).HasMaxLength(100);
			Property(plp => plp.Description).HasMaxLength(2000);

			HasRequired(plp => plp.PropertyListing)
				.WithMany(pl => pl.Photos)
				.HasForeignKey(plp => plp.PropertyListingID)
				.WillCascadeOnDelete(false);

			HasOptional(plp => plp.CreatorSession)
				.WithMany()
				.HasForeignKey(plp => plp.CreatorSessionID)
				.WillCascadeOnDelete(false);

			HasOptional(plp => plp.CreatorUser)
				.WithMany()
				.HasForeignKey(plp => plp.CreatorUserID)
				.WillCascadeOnDelete(false);
		}
	}
}