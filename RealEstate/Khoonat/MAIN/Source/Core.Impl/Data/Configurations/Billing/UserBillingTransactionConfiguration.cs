using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class UserBillingTransactionConfiguration : EntityTypeConfiguration<UserBillingTransaction>
    {
        public UserBillingTransactionConfiguration()
        {
            HasRequired(e => e.User)
                .WithMany(u => u.BillingTransactions)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            Property(e => e.CashDelta).HasPrecision(18, 2);
            Property(e => e.BonusDelta).HasPrecision(18, 2);

            Property(e => e.CashBalance).HasPrecision(18, 2);
            Property(e => e.BonusBalance).HasPrecision(18, 2);

            Property(e => e.CashTurnover).HasPrecision(18, 2);
            Property(e => e.BonusTurnover).HasPrecision(18, 2);
        }
    }
}