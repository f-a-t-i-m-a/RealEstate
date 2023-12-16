using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    public class MoveVicinitiesInput
    {
        public List<string> Ids { get; set; }
        public string ParentId { get; set; }
    }
}
