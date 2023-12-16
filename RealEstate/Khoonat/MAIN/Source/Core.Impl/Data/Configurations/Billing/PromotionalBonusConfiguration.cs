using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class PromotionalBonusConfiguration : BillingSourceEntityConfiguration<PromotionalBonus>
    {
        public PromotionalBonusConfiguration()
        {
            HasOptional(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserID)
                .WillCascadeOnDelete(false);

            Property(e => e.Description).HasMaxLength(2000);
            Property(e => e.BonusAmount).HasPrecision(18, 2);
        }
    }
}