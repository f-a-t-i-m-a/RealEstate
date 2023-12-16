using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedPropertySearchEmailNotificationConfiguration : EntityTypeConfiguration<SavedPropertySearchEmailNotification>
    {
        public SavedPropertySearchEmailNotificationConfiguration()
        {
            HasRequired(en => en.PropertyListing)
                .WithMany()
                .HasForeignKey(en => en.PropertyListingID)
                .WillCascadeOnDelete(false);

            HasRequired(en => en.SavedSearch)
                .WithMany()
                .HasForeignKey(en => en.SavedSearchID)
                .WillCascadeOnDelete(false);

            HasRequired(en => en.ContactMethod)
                .WithMany()
                .HasForeignKey(en => en.ContactMethodID)
                .WillCascadeOnDelete(false);
        }
    }
}