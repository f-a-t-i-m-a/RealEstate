using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [TsClass]
    public class PrintContractInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
    }
}