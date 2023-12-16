using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredPropertyAdmin
{
    public class SponsoredPropertyAdminReviewActionModel
    {
        public PropertyListingDetails PropertyListingDetails { get; set; }
        public PropertyListing PropertyListing { get; set; }
        public PropertyListingSummary ListingSummary { get; set; }
        public SponsoredPropertyListing SponsoredPropertyListing { get; set; }
    }
}