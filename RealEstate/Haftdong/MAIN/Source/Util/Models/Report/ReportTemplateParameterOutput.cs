using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    public class ReportTemplateParameterOutput
    {
        public List<ReportTemplateParameterSummary> ReportTemplateParameterSummaries { get; set; }
    }
}