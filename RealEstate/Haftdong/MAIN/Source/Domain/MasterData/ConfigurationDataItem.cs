using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.MasterData
{
    public class ConfigurationDataItem
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
    }
}