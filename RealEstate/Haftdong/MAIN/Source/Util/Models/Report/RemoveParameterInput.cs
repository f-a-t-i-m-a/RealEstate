using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    [TsClass]
    public class RemoveParameterInput
    {
        [BsonId]
        public ObjectId ReportTemplateID { get; set; }
        public ObjectId ParameterID { get; set; }
        
    }
}