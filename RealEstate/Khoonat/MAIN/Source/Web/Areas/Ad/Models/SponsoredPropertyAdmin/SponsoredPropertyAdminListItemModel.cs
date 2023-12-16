using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredPropertyAdmin
{
    public class SponsoredPropertyAdminListItemModel
    {
        public long ID { get; set; }

        public long SponsoredEntityID { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }
        public User BilledUser { get; set; }

        public long ListingID { get; set; }
        public PropertyListing Listing { get; set; }

        public bool ShowInAllPages { get; set; }
        public bool ShowOnMap { get; set; }
        public bool? Approved { get; set; }

        public int ClicksCount { get; set; }
        public int ImpressionsCount { get; set; }
    }
}