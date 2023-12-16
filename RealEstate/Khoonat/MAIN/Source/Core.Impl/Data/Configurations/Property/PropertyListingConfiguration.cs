using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property
{
	public class PropertyListingConfiguration : EntityTypeConfiguration<PropertyListing>
	{
		public PropertyListingConfiguration()
		{
			Ignore(pl => pl.Content);
			Property(pl => pl.ContentString).HasMaxLength(null);

			Property(pl => pl.Code).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
		    Property(pl => pl.VicinityHierarchyString).HasMaxLength(150);
			Property(pl => pl.AppropriateVisitTimes).HasMaxLength(500);
			Property(pl => pl.InappropriateVisitTimes).HasMaxLength(500);
			Property(pl => pl.HowToCoordinateBeforeVisit).HasMaxLength(500);
			Property(pl => pl.EditPassword).HasMaxLength(20);

			Property(sc => sc.NormalizedPrice).HasPrecision(17, 2);
			Property(sc => sc.NormalizedPricePerEstateArea).HasPrecision(17, 2);
			Property(sc => sc.NormalizedPricePerUnitArea).HasPrecision(17, 2);

            HasOptional(pl => pl.Vicinity)
                .WithMany(v => v.PropertyListings)
                .HasForeignKey(pl => pl.VicinityID)
                .WillCascadeOnDelete(false);

			HasOptional(pl => pl.Estate)
				.WithMany()
				.HasForeignKey(pl => pl.EstateID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.Building)
				.WithMany()
				.HasForeignKey(pl => pl.BuildingID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.Unit)
				.WithMany()
				.HasForeignKey(pl => pl.UnitID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.SaleConditions)
				.WithMany()
				.HasForeignKey(pl => pl.SaleConditionsID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.RentConditions)
				.WithMany()
				.HasForeignKey(pl => pl.RentConditionsID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.ContactInfo)
				.WithMany()
				.HasForeignKey(pl => pl.ContactInfoID)
				.WillCascadeOnDelete(false);

			HasOptional(pl => pl.CreatorSession)
				.WithMany()
				.HasForeignKey(pl => pl.CreatorSessionID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.CreatorUser)
				.WithMany()
				.HasForeignKey(pl => pl.CreatorUserID)
				.WillCascadeOnDelete(false);
			HasOptional(pl => pl.OwnerUser)
				.WithMany()
				.HasForeignKey(pl => pl.OwnerUserID)
				.WillCascadeOnDelete(false);
		}
	}
}