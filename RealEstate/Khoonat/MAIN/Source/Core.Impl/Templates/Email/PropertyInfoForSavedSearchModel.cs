using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.SavedSearch;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class PropertyInfoForSavedSearchModel
	{
		// Components
		public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

		// Data
		public User User { get; set; }
		public PropertyListingDetails Listing { get; set; }
		public PropertyListingSummary ListingSummary { get; set; }
		public SavedPropertySearch SavedSearch { get; set; }
	}
}