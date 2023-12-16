
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredProperty
{
    public class SponsoredPropertyConfirmationPopupModel
    {
        public PropertyListingSummary ListingSummary { get; set; }
        public long SponsoredEntityID { get; set; }
        public bool Enabled { get; set; }
        public string ReferringController { get; set; }

    }
}