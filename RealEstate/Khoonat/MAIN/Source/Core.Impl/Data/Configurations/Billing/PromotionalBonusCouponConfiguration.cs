using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class PromotionalBonusCouponConfiguration : BillingSourceEntityConfiguration<PromotionalBonusCoupon>
    {
        public PromotionalBonusCouponConfiguration()
        {
            HasRequired(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserID)
                .WillCascadeOnDelete(false);

            Property(e => e.CouponValue).HasPrecision(18, 2);
            Property(e => e.CouponNumber).HasMaxLength(20);
            Property(e => e.CouponPassword).HasMaxLength(20);
        }
    }
}