using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.MasterData
{
    public class ReportTemplateParameter
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string ParameterName { get; set; }
        public string DisplayText { get; set; }
        public ParameterType ParameterType { get; set; }
        public string DefaultValue { get; set; }
        public string Max { get; set; }
        public string Min { get; set; }
        public bool Required { get; set; }
    }
}