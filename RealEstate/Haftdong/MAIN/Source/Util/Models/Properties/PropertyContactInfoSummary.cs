using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    public class PropertyContactInfoSummary
    {
        public List<SupplyContactInfoSummary> ContactInfos { get; set; }
        public CustomerSummary Owner { get; set; }
    }
}