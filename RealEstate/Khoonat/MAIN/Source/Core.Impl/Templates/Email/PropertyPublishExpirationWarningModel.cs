
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{

    public class PropertyPublishExpirationWarningModel
	{
        public string OwnerName { get; set; }
        public PropertyListing Listing { get; set; }
        public PropertyListingSummary ListingSummary { get; set; }
	}
}
