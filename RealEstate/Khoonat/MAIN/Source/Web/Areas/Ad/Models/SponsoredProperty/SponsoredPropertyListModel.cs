using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredProperty
{
    // [Bind(Exclude = "SponsoredProperties")]
    public class SponsoredPropertyListModel
    {
        public PagedList<SponsoredPropertyListing> SponsoredProperties { get; set; }
        public string Page { get; set; }
    }
}