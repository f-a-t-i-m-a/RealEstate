using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad
{
    public class SponsoredEntityImpressionBillingConfiguration : BillingSourceEntityConfiguration<SponsoredEntityImpressionBilling>
    {
        public SponsoredEntityImpressionBillingConfiguration()
        {
            HasRequired(seib => seib.SponsoredEntity)
                .WithMany(se => se.ImpressionBillings)
                .HasForeignKey(seib => seib.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            Property(seib => seib.TotalAmount).HasPrecision(18, 2);

            HasMany(seib => seib.Impressions)
                .WithOptional(sei => sei.BillingEntity)
                .HasForeignKey(sei => sei.BillingEntityID)
                .WillCascadeOnDelete(false);
        }
    }
}