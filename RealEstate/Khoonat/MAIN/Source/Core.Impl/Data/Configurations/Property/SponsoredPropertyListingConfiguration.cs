using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
    public class SponsoredPropertyListingConfiguration : EntityTypeConfiguration<SponsoredPropertyListing>
    {
        public SponsoredPropertyListingConfiguration()
        {
            HasRequired(spl => spl.SponsoredEntity)
                .WithMany()
                .HasForeignKey(spl => spl.SponsoredEntityID)
                .WillCascadeOnDelete(false);

            HasRequired(spl => spl.Listing)
                .WithMany(pl => pl.Sponsorships)
                .HasForeignKey(spl => spl.ListingID)
                .WillCascadeOnDelete(false);

            Property(spl => spl.CustomCaption).HasMaxLength(200);
        }
    }
}