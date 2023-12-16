using AutoMapper;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
    public class ApiSponsoredPropertyListingModel
    {
        public long ID { get; set; }
        public bool? Approved { get; set; }

        public long SponsoredEntityID { get; set; }
        public ApiSponsoredEntityModel SponsoredEntity { get; set; }

        public long ListingID { get; set; }
        public ApiPropertyDetailsModel Listing { get; set; }

        public bool IgnoreSearchQuery { get; set; }
        public bool ShowInAllPages { get; set; }
        public bool ShowOnMap { get; set; }

        public string CustomCaption { get; set; }

        public static void ConfigureMapper()
        {
            Mapper.CreateMap<SponsoredPropertyListing, ApiSponsoredPropertyListingModel>();
        }
    }
}