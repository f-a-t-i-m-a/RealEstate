using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class PropertyListingPublishHistoryConfiguration : EntityTypeConfiguration<PropertyListingPublishHistory>
	{
		public PropertyListingPublishHistoryConfiguration()
		{
			HasRequired(h => h.PropertyListing)
				.WithMany()
				.HasForeignKey(h => h.PropertyListingID)
				.WillCascadeOnDelete(true);

			HasOptional(h => h.Session)
				.WithMany()
				.HasForeignKey(h => h.SessionID)
				.WillCascadeOnDelete(false);

			HasOptional(h => h.User)
				.WithMany()
				.HasForeignKey(h => h.UserID)
				.WillCascadeOnDelete(false);
		}
	}
}