using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    public class ReportListOutput
    {
        public List<ReportTemplateSummary> Templates { get; set; }
    }
}