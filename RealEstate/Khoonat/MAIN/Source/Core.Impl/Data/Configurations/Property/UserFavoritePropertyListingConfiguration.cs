using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class UserFavoritePropertyListingConfiguration : EntityTypeConfiguration<UserFavoritePropertyListing>
	{
		public UserFavoritePropertyListingConfiguration()
		{
			HasRequired(f => f.CreationSession)
				.WithMany()
				.HasForeignKey(f => f.CreationSessionID)
				.WillCascadeOnDelete(false);

			HasRequired(f => f.Listing)
				.WithMany(l => l.FavoritedBy)
				.HasForeignKey(f => f.ListingID)
				.WillCascadeOnDelete(true);

			HasRequired(f => f.User)
				.WithMany(u => u.FavoritePropertyListings)
				.HasForeignKey(f => f.UserID)
				.WillCascadeOnDelete(true);
		}
	}
}