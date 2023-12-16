using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Naroon.Models
{
    public class XmlDoc
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime FetchDateFrom { get; set; } 
        public DateTime FetchDateTo { get; set; } 
        public string DataContent { get; set; } 
        public string SubDataContent { get; set; } 
    }
}
