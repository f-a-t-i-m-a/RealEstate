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
    public class UpdateReportTemplateInput
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public byte[] Definition { get; set; }
        public long Order { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ReportTemplate, UpdateReportTemplateInput>();
        }
    }
}