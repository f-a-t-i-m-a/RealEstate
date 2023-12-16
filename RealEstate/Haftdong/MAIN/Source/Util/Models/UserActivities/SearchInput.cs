using System;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.UserActivities
{
    [TsClass]
    public class SearchInput
    {
        public string CorrelationId { get; set; }
        public bool? HasTargetState { get; set; }
        public ApplicationType? ApplicationType { get; set; }
        public bool? HasComment { get; set; }
        public bool? AllActivity { get; set; }
        public ObjectId? UserId { get; set; }
        public string Controller { get; set; }
        public string ActionName { get; set; }
        public EntityType? TargetType { get; set; }
        public ObjectId? TargetId { get; set; }
        public string TargetState { get; set; }
        public bool? Succeeded { get; set; }
        public UserActivityType? ActivityType { get; set; }
        public string ActivitySubType { get; set; }
        public EntityType? ParentType { get; set; }
        public ObjectId? ParentId { get; set; }
        public EntityType? DetailType { get; set; }
        public ObjectId? DetailId { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public UserActivitySortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
        public DateTime? FromActivityTime { get; set; }
        public int? FromActivityTimeMinute { get; set; }
        public int? FromActivityTimeHour { get; set; }
        public DateTime? ToActivityTime { get; set; }
        public int? ToActivityTimeMinute { get; set; }
        public int? ToActivityTimeHour { get; set; }
    }
}