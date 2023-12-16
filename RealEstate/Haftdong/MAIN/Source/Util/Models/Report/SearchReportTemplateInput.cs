using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    [AutoMapperConfig]
    public class SearchReportTemplateInput
    {
        public ReportDataSourceType DataSourceType { get; set; }
        public ApplicationImplementedReportDataSourceType? ApplicationImplementedDataSourceType { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public long Order { get; set; }
        public byte[] Definition { get; set; }
        public List<ReportTemplateParameter> Parameters { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<SearchReportTemplateInput, ReportTemplate>()
                .IgnoreUnmappedProperties();
        }
    }
}