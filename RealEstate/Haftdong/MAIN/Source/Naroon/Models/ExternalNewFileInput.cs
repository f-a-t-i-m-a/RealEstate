using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;

namespace JahanJooy.RealEstateAgency.Naroon.Models
{
    public class ExternalNewFileInput
    {
        public Property Property { get; set; }
        public Supply Supply { get; set; }
    }
}
