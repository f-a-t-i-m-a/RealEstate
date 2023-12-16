using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.Property
{
    public class ViewPropertyDetailsMapTabModel
    {
        public string Url { get; set; }
        public Vicinity Vicinity { get; set; }
        public PropertyListingSummary ListingSummary { get; set; }
    }
}