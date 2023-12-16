using System.Collections.Generic;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    public class ReportPrintInput
    {
        public long TemplateID { get; set; }
        public string Format { get; set; }
        public List<ParameterValue> Parameters { get; set; }
    }
}