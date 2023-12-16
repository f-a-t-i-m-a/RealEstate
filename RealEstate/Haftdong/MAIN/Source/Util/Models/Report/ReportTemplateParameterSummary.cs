using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [AutoMapperConfig]
    public class ReportTemplateParameterSummary
    {
        public ObjectId ID { get; set; }
        public string ParameterName { get; set; }
        public string DisplayText { get; set; }
        public ParameterType ParameterType { get; set; }
        public string DefaultValue { get; set; }
        public string Max { get; set; }
        public string Min { get; set; }
        public bool Required { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ReportTemplateParameter, ReportTemplateParameterSummary>();
        }
    }
}