using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    public class AddParameterInput
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public ReportTemplateParameter Parameter { get; set; }
        
    }
}