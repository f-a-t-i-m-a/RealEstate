using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedSearchSmsNotificationBillingConfiguration : BillingSourceEntityConfiguration<SavedSearchSmsNotificationBilling>
    {
        public SavedSearchSmsNotificationBillingConfiguration()
        {
            HasMany(b => b.PropertyNotifications)
                .WithOptional(n => n.Billing)
                .HasForeignKey(n => n.BillingID)
                .WillCascadeOnDelete(false);
        }
    }
}