using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    public class PrintSuppliesInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
        public SearchFileInput SearchInput { get; set; }
        public List<string> Ids { get; set; }
    }
}