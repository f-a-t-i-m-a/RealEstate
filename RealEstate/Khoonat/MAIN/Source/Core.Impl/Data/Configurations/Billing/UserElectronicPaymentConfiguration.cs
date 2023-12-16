using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class UserElectronicPaymentConfiguration : BillingSourceEntityConfiguration<UserElectronicPayment>
    {
        public UserElectronicPaymentConfiguration()
        {
            Property(e => e.Amount).HasPrecision(18, 2);

            Property(e => e.BankAmount).HasPrecision(18, 2);
            Property(e => e.BankInvoiceNumber).HasMaxLength(30);
            Property(e => e.BankInvoiceDate).HasMaxLength(30);
            Property(e => e.BankTransactionDate).HasMaxLength(25);
            Property(e => e.BankMerchantCode).HasMaxLength(30);
            Property(e => e.BankTerminalCode).HasMaxLength(30);
           
            Property(e => e.BankVerifyPaymentResultMessage).HasMaxLength(250);
        }
    }
}