using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services.Dto
{
    public class SponsoredPropertyListingSummary
	{
        public PropertyListingSummary PropertyListingSummary { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }
        public SponsoredPropertyListing SponsoredPropertyListing { get; set; }
        public SponsoredEntityImpression SponsoredEntityImpression { get; set; }
	}
}