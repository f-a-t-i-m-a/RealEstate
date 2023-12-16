using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    public class ListOfVicinityInput
    {
        public List<string> Ids { get; set; }
        public bool Value { get; set; }
    }
}
