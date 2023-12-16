using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad
{
    public class SponsoredEntityClickConfiguration : EntityTypeConfiguration<SponsoredEntityClick>
    {
        public SponsoredEntityClickConfiguration()
        {
            HasRequired(sec => sec.SponsoredEntity)
                .WithMany(se => se.Clicks)
                .HasForeignKey(sec => sec.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasRequired(sec => sec.Impression)
                .WithMany()
                .HasForeignKey(sec => sec.ImpressionID)
                .WillCascadeOnDelete(false);

            HasRequired(sec => sec.HttpSession)
                .WithMany()
                .HasForeignKey(sec => sec.HttpSessionID)
                .WillCascadeOnDelete(false);

            HasOptional(sec => sec.BillingEntity)
                .WithMany(secb => secb.Clicks)
                .HasForeignKey(sec => sec.BillingEntityID)
                .WillCascadeOnDelete(false);
        }
    }
}