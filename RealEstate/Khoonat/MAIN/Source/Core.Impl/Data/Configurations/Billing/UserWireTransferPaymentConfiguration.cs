using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class UserWireTransferPaymentConfiguration : BillingSourceEntityConfiguration<UserWireTransferPayment>
    {
        public UserWireTransferPaymentConfiguration()
        {
            HasOptional(e => e.ReviewedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReviewedByUserID)
                .WillCascadeOnDelete(false);

            Property(e => e.Amount).HasPrecision(18, 2);

            Property(e => e.SourceCardNumberLastDigits).HasMaxLength(10);
            Property(e => e.SourceAccountHolderName).HasMaxLength(80);
            Property(e => e.UserEnteredDescription).HasMaxLength(500);
            Property(e => e.FollowUpNumber).HasMaxLength(50);
        }
    }
}