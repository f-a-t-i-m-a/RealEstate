using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
    public class VicinityConfiguration : EntityTypeConfiguration<Vicinity>
    {
        public VicinityConfiguration()
        {
            Property(v => v.Name).IsRequired().HasMaxLength(80);
            Property(v => v.AlternativeNames).IsOptional().HasMaxLength(300);
            Property(v => v.AdditionalSearchText).IsOptional().HasMaxLength(300);
            Property(v => v.Description).IsOptional().HasMaxLength(500);
	        Property(v => v.OfficialLinkUrl).IsOptional().HasMaxLength(1000);
	        Property(v => v.WikiLinkUrl).IsOptional().HasMaxLength(1000);
            Property(v => v.AdministrativeNotes).IsOptional().HasMaxLength(500);

            HasOptional(v => v.Parent)
                .WithMany(v => v.Children)
                .HasForeignKey(v => v.ParentID)
                .WillCascadeOnDelete(false);

            HasMany(v => v.PropertyListings)
                .WithOptional(pl => pl.Vicinity)
                .HasForeignKey(pl => pl.VicinityID)
                .WillCascadeOnDelete(false);
        }
    }
}