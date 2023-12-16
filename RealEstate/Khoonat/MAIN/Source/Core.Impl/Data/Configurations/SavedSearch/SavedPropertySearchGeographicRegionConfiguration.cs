using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch
{
	public class SavedPropertySearchGeographicRegionConfiguration : EntityTypeConfiguration<SavedPropertySearchGeographicRegion>
	{
		public SavedPropertySearchGeographicRegionConfiguration()
		{
			HasRequired(sgr => sgr.PropertySearch)
				.WithMany(ps => ps.GeographicRegions)
				.HasForeignKey(sgr => sgr.PropertySearchID)
				.WillCascadeOnDelete(true);

			HasRequired(sgr => sgr.Vicinity)
				.WithMany()
				.HasForeignKey(sgr => sgr.VicinityID)
				.WillCascadeOnDelete(true);
		}
	}
}