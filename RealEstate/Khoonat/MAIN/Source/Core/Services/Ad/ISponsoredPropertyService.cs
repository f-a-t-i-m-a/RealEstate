using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services.Ad
{
    [Contract]
    public interface ISponsoredPropertyService
    {
        ValidationResult CreateSponsoredProperty(SponsoredEntity sponsoredEntity, SponsoredPropertyListing sponsoredPropertyListing);
        ValidationResult UpdateSponsoredProperty(SponsoredEntity sponsoredEntity, SponsoredPropertyListing sponsoredPropertyListing);
        void SetApproved(long listingId, bool? approved);
        void EditCustomCaption(long listingId, string customCaption);
    }
}