using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
    public class SavedPropertySearchPromotionalSmsNotDeliveredReturnConfiguration : BillingSourceEntityConfiguration<SavedPropertySearchPromotionalSmsNotDeliveredReturn>
    {
        public SavedPropertySearchPromotionalSmsNotDeliveredReturnConfiguration()
        {
            HasRequired(e => e.PropertyListing)
                .WithMany()
                .HasForeignKey(e => e.PropertyListingID)
                .WillCascadeOnDelete(false);

            HasRequired(e => e.PromotionalSms)
                .WithMany()
                .HasForeignKey(e => e.PromotionalSmsID)
                .WillCascadeOnDelete(false);
        }
    }
}