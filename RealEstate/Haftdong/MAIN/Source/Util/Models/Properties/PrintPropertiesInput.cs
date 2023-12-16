using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    public class PrintPropertiesInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
        public SearchPropertyInput SearchInput { get; set; }
        public List<string> Ids { get; set; }
    }
}