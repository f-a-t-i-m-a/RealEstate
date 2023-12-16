using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad
{
    public class SponsoredEntityConfiguration : EntityTypeConfiguration<SponsoredEntity>
    {
        public SponsoredEntityConfiguration()
        {
            HasRequired(se => se.BilledUser)
                .WithMany()
                .HasForeignKey(se => se.BilledUserID)
                .WillCascadeOnDelete(false);

            Property(se => se.Title).HasMaxLength(100);
            Property(se => se.MaxPayPerImpression).HasPrecision(18, 2);
            Property(se => se.MaxPayPerClick).HasPrecision(18, 2);
            Property(se => se.EstimatedClicksPerImpression).HasPrecision(18, 10);
            Property(se => se.ProjectedMaxPayPerImpression).HasPrecision(18, 2);

            HasMany(se => se.Impressions)
                .WithRequired(sei => sei.SponsoredEntity)
                .HasForeignKey(sei => sei.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasMany(se => se.ImpressionBillings)
                .WithRequired(seib => seib.SponsoredEntity)
                .HasForeignKey(seib => seib.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasMany(se => se.Clicks)
                .WithRequired(sec => sec.SponsoredEntity)
                .HasForeignKey(sec => sec.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasMany(se => se.ClickBillings)
                .WithRequired(secb => secb.SponsoredEntity)
                .HasForeignKey(secb => secb.SponsoredEntityID)
                .WillCascadeOnDelete(false);
        }
    }
}