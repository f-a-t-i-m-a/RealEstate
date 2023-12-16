using System;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Audit
{
    public class UserActivity
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string CorrelationID { get; set; }
        public bool IsMainActivity { get; set; }
        public ApplicationType ApplicationType { get; set; }

        public DateTime ActivityTime { get; set; }
        public ApplicationUserReference User { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool Succeeded { get; set; }

        public EntityType? TargetType { get; set; }
        public ObjectId? TargetID { get; set; }
        public string TargetState { get; set; }
        public UserActivityType ActivityType { get; set; }
        public string ActivitySubType { get; set; }

        public EntityType? ParentType { get; set; }
        public EntityType? DetailType { get; set; }
        public ObjectId? ParentID { get; set; }
        public ObjectId? DetailID { get; set; }
        public string Comment { get; set; }

    }
}