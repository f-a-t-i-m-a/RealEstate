using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    public class PrintSupplyInput
    {
        public string ReportTemplateID { get; set; }
        public string Format { get; set; }
    }
}