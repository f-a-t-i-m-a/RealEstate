using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedPropertySearchSmsNotificationConfiguration : EntityTypeConfiguration<SavedPropertySearchSmsNotification>
    {
        public SavedPropertySearchSmsNotificationConfiguration()
        {
            HasRequired(sn => sn.PropertyListing)
                .WithMany()
                .HasForeignKey(en => en.PropertyListingID)
                .WillCascadeOnDelete(false);

            HasRequired(sn => sn.SavedSearch)
                .WithMany()
                .HasForeignKey(en => en.SavedSearchID)
                .WillCascadeOnDelete(false);

            HasRequired(sn => sn.ContactMethod)
                .WithMany()
                .HasForeignKey(en => en.ContactMethodID)
                .WillCascadeOnDelete(false);
            
            HasRequired(sn => sn.TargetUser)
                .WithMany()
                .HasForeignKey(e => e.TargetUserID)
                .WillCascadeOnDelete(false);

            HasOptional(sn => sn.Billing)
                .WithMany(b => b.PropertyNotifications)
                .HasForeignKey(sn => sn.BillingID)
                .WillCascadeOnDelete(false);
        }
    }
}