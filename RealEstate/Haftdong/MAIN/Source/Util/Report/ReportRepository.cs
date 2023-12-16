using System.Drawing.Printing;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Units;

namespace JahanJooy.RealEstateAgency.Util.Report
{
    [Contract]
    [Component]
    public class ReportRepository
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

//        [ComponentPlug]
//        public DirectDbReport DirectDbReport { get; set; }

//        [ComponentPlug]
//        public GeneralReport GeneralReport { get; set; }

        public StiReport GetApplicationImplemented(ApplicationImplementedReportDataSourceType dataSourceType, string reportTemplateId)
        {
            ReportTemplate template;

            if (!string.IsNullOrEmpty(reportTemplateId))
            {
                var filter = Builders<ReportTemplate>.Filter.Eq("ID", ObjectId.Parse(reportTemplateId));
                filter &= Builders<ReportTemplate>.Filter.Eq("DataSourceType", ReportDataSourceType.ApplicationImplemented);
                filter &= Builders<ReportTemplate>.Filter.Eq("ApplicationImplementedDataSourceType", dataSourceType);
                template = DbManager.ReportTemplate.Find(filter).SingleOrDefaultAsync().Result;
            }
            else
            {
                var filter = Builders<ReportTemplate>.Filter.Eq("DataSourceType", ReportDataSourceType.ApplicationImplemented);
                filter &= Builders<ReportTemplate>.Filter.Eq("ApplicationImplementedDataSourceType", dataSourceType);
                template = DbManager.ReportTemplate.Find(filter).SingleOrDefaultAsync().Result;
            }

            if (template == null)
                return null;

            var report = new StiReport();
            report.Load(template.Definition);
            report.Dictionary.Databases.Clear(); // Make sure no unwanted connection strings present on the report

            CanonicalizeReport(report);
            return report;
        }

//        public StiReport GetDirectDbConnection(long reportTemplateId, List<ParameterValue> parameters)
//        {
//            var template = DbManager.Db.ReportTemplate
//                .SingleOrDefault(rt =>
//                    rt.ID == reportTemplateId &&
//                    rt.DataSourceType == ReportDataSourceType.DirectDbConnection);
//
//            if (template == null)
//                return null;
//            var reportParameters = DbManager.Db.ReportTemplateParameter.
//                Where(r => r.ReportTemplateID == reportTemplateId).ToList();
//
//            foreach (var parameter in parameters)
//            {
//                var reportTemplateParameter = reportParameters.SingleOrDefault(r => r.ParameterName == parameter.ParameterName);
//                if (reportTemplateParameter != null)
//                {
//                    parameter.ParameterType = reportTemplateParameter.ParameterType;
//
//                    if (reportTemplateParameter.ParameterType == ParameterType.DateTime && reportTemplateParameter.Max != "")
//                    {
//                        if (Convert.ToDateTime(parameter.Value) > Convert.ToDateTime(reportTemplateParameter.Max))
//                            throw new ArgumentOutOfRangeException(reportTemplateParameter.ParameterName);
//                    }
//                    if (reportTemplateParameter.ParameterType == ParameterType.DateTime && reportTemplateParameter.Min != "")
//                    {
//                        if (Convert.ToDateTime(parameter.Value) < Convert.ToDateTime(reportTemplateParameter.Min))
//                            throw new ArgumentOutOfRangeException(reportTemplateParameter.ParameterName);
//                    }
//                }
//            }
//            var parametersOutput = new ParametersOutput(parameters);
//            var report = new StiReport();
//            report.Load(template.Definition);
//            GeneralReport.PopulateVariables(report, reportParameters, parametersOutput);
//            DirectDbReport.PopulateConnectionStrings(report);
//
//            CanonicalizeReport(report);
//            return report;
//        }

        public StiReport NewEmptyReport()
        {
            var report = new StiReport();

            foreach (StiPage page in report.Pages)
            {
                page.PaperSize = PaperKind.A4;
                page.RightToLeft = true;
            }

            report.Unit = StiUnit.Millimeters;

            CanonicalizeReport(report);

            return report;
        }

        public void CanonicalizeReport(StiReport report)
        {
            SetReferences(report);
        }

        #region Private helper methods

        private static void SetReferences(StiReport report)
        {
            report.ReferencedAssemblies = report.ReferencedAssemblies.Union(new[]
            {
                "JahanJooy.Stimulsoft.Common",
                "JahanJooy.RealEstateAgency.HaftDong",
                "JahanJooy.RealEstateAgency.Domain",
                "JahanJooy.Common.Util",
                "JahanJooy.RealEstateAgency.LibraryBuffer",
                "MongoDB.Bson"
            }).Distinct().ToArray();
        }

        #endregion

    }
}