using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.Models.Report;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public class ReportDesignerParams
    {
        public ReportDataSourceType DataSourceType { get; set; }
        public ApplicationImplementedReportDataSourceType? ApplicationImplementedDataSourceType { get; set; }
        public ParametersOutput ReportParameters { get; set; }
        public string ID { get; set; }
    }
}