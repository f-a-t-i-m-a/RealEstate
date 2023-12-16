using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class PropertyRegistrationInfoForOwnerModel
	{
        // Components
        public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

        // Data
        public PropertyListingSummary ListingSummary { get; set; }
        public PropertyListingContactInfo ContactInfo { get; set; }
        public string ListingEditPassword { get; set; }
	}
}
