using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.UserActivities
{
    [TsClass]
    [AutoMapperConfig]
    public class UserActivitySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime ActivityTime { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public ApplicationUserReference User { get; set; }
        public string ControllerName { get; set; }
        public string CorrelationID { get; set; }
        public string ActionName { get; set; }
        public bool Succeeded { get; set; }
        public EntityType? TargetType { get; set; }
        public string TargetState { get; set; }
        public UserActivityType ActivityType { get; set; }
        public string ActivitySubType { get; set; }
        public EntityType? ParentType { get; set; }
        public EntityType? DetailType { get; set; }
        public string Comment { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UserActivity, UserActivitySummary>()
                .IgnoreUnmappedProperties();
        }
    }
}