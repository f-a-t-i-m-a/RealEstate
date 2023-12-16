using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Audit;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.UserActivities
{
    [TsClass]
    [AutoMapperConfig]
    public class UserActivityDetails : UserActivitySummary
    {
        public ObjectId? TargetID { get; set; }
        public ObjectId? ParentID { get; set; }
        public ObjectId? DetailID { get; set; }
        public List<UserActivitySummary> RelativeActivities { get; set; } 

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UserActivity, UserActivityDetails>()
                .IgnoreUnmappedProperties();
        }
    }
}