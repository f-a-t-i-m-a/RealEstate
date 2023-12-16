using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.MasterData
{
    public class ReportTemplate
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
    }
}