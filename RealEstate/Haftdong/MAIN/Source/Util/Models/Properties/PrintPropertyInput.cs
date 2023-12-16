using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    public class PrintPropertyInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
    }
}