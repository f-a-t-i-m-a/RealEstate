using System;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    public class PublishSupplyInput
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public NewContactMethodCollectionInput OwnerContact { get; set; }
        public NewContactMethodCollectionInput AgencyContact { get; set; }
    }
}