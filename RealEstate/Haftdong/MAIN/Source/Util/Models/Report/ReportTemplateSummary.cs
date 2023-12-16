using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    [AutoMapperConfig]
    public class ReportTemplateSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
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
            Mapper.CreateMap<ReportTemplate, ReportTemplateSummary>();
            Mapper.CreateMap<ReportTemplateSummary, ReportTemplate>();
        }
    }
}