using System.Collections.Generic;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredProperty
{
    public class SponsoredPropertyViewSponsorshipStatusModel
    {
        public PropertyListing PropertyListing { get; set; }
        public List<SponsoredPropertyListing> Sponsorships { get; set; }
    }
}