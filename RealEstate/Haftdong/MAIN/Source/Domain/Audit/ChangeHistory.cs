using System;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Audit
{
    public class ChangeHistory
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string CorrelationID { get; set; }

        public DateTime ChangeTime { get; set; }
        public ApplicationUserReference User { get; set; }
        
        public EntityType TargetType { get; set; }
        public ObjectId TargetID { get; set; }
        public ChangeHistoryChangeType ChangeType { get; set; }
        public string ChangeDetails { get; set; }
    }
}