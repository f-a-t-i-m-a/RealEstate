using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class UserRefundRequestConfiguration : BillingSourceEntityConfiguration<UserRefundRequest>
    {
        public UserRefundRequestConfiguration()
        {
            HasOptional(e => e.ReviewedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReviewedByUserID)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.ClearedByUser)
                .WithMany()
                .HasForeignKey(e => e.ClearedByUserID)
                .WillCascadeOnDelete(false);

            Property(e => e.RequestedAmount).HasPrecision(18, 2);
            Property(e => e.DeductibleBankTransactionFee).HasPrecision(18, 2);
            Property(e => e.PayableAmount).HasPrecision(18, 2);

            Property(e => e.TargetCardNumber).HasMaxLength(25);
            Property(e => e.TargetShebaNumber).HasMaxLength(35);
            Property(e => e.TargetAccountHolderName).HasMaxLength(80);
            Property(e => e.UserEnteredReason).HasMaxLength(500);
            Property(e => e.UserEnteredDescription).HasMaxLength(500);
        }
    }
}