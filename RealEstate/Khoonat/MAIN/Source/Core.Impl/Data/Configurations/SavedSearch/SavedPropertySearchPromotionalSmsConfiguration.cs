using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedPropertySearchPromotionalSmsConfiguration : BillingSourceEntityConfiguration<SavedPropertySearchPromotionalSms>
    {
        public SavedPropertySearchPromotionalSmsConfiguration()
        {
            HasRequired(e => e.PropertyListing)
                .WithMany()
                .HasForeignKey(e => e.PropertyListingID)
                .WillCascadeOnDelete(false);

            Property(e => e.MessageText).HasMaxLength(300);
        }
    }
}