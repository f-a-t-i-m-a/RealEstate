using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    public class PrintRequestsInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
        public SearchRequestInput SearchInput { get; set; }
        public List<string> Ids { get; set; }
    }
}