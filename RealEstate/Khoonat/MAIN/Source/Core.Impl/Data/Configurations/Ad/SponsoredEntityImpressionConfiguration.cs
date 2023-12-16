using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad
{
    public class SponsoredEntityImpressionConfiguration : EntityTypeConfiguration<SponsoredEntityImpression>
    {
        public SponsoredEntityImpressionConfiguration()
        {
            HasRequired(sei => sei.SponsoredEntity)
                .WithMany(se => se.Impressions)
                .HasForeignKey(sei => sei.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasOptional(sei => sei.ContentOwnerUser)
                .WithMany()
                .HasForeignKey(sei => sei.ContentOwnerUserID)
                .WillCascadeOnDelete(false);

            HasRequired(sei => sei.HttpSession)
                .WithMany()
                .HasForeignKey(sei => sei.HttpSessionID)
                .WillCascadeOnDelete(false);

            HasOptional(sei => sei.BillingEntity)
                .WithMany(seib => seib.Impressions)
                .HasForeignKey(sei => sei.BillingEntityID)
                .WillCascadeOnDelete(false);

            Property(sei => sei.BidAmount).HasPrecision(18, 2);
        }
    }
}