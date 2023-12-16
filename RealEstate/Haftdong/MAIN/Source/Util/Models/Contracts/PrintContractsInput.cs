using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [TsClass]
    public class PrintContractsInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
        public SearchContractInput SearchInput { get; set; }
        public List<string> Ids { get; set; }
    }
}