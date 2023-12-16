using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public class UserBalanceAdministrativeChangeConfiguration : BillingSourceEntityConfiguration<UserBalanceAdministrativeChange>
    {
        public UserBalanceAdministrativeChangeConfiguration()
        {
            HasRequired(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserID)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.ReviewedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReviewedByUserID)
                .WillCascadeOnDelete(false);

            Property(e => e.Description).HasMaxLength(2000);
            Property(e => e.AdministrativeNotes).HasMaxLength(2000);
            Property(e => e.CashDelta).HasPrecision(18, 2);
            Property(e => e.BonusDelta).HasPrecision(18, 2);
        }
    }
}