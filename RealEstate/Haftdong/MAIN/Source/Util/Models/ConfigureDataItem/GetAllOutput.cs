using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem
{
    [TsClass]
    public class GetAllOutput
    {
        public List<ConfigureDataItemSummary> Items { get; set; }
    }
}