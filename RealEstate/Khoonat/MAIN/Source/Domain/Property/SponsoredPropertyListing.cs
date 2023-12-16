using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Domain.Property
{
    public class SponsoredPropertyListing : ISponsoredEntityRelated
    {
        public long ID { get; set; }
        public bool? Approved { get; set; }

        public long SponsoredEntityID { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }

        public long ListingID { get; set; }
        public PropertyListing Listing { get; set; }

        public bool IgnoreSearchQuery { get; set; }
        public bool ShowInAllPages { get; set; }
        public bool ShowOnMap { get; set; }

        public string CustomCaption { get; set; }
    }
}