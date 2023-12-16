using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad
{
    public class SponsoredEntityClickBillingConfiguration : BillingSourceEntityConfiguration<SponsoredEntityClickBilling>
    {
        public SponsoredEntityClickBillingConfiguration()
        {
            HasRequired(secb => secb.SponsoredEntity)
                .WithMany(se => se.ClickBillings)
                .HasForeignKey(secb => secb.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            Property(secb => secb.TotalAmount).HasPrecision(18, 2);

            HasMany(secb => secb.Clicks)
                .WithOptional(sec => sec.BillingEntity)
                .HasForeignKey(sei => sei.BillingEntityID)
                .WillCascadeOnDelete(false);
        }
    }
}