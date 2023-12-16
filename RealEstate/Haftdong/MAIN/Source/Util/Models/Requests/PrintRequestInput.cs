using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    public class PrintRequestInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
    }
}