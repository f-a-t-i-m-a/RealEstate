using System;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UserActivityAttribute : Attribute
    {
        public UserActivityAttribute(UserActivityType activityType, EntityType targetType, ApplicationType applicationType)
        {
            ActivityType = activityType;
            TargetType = targetType;
            ApplicationType = applicationType;
        }

        public UserActivityType ActivityType { get; }
        public EntityType TargetType { get; }
        public ApplicationType ApplicationType { get; }

        public object ActivitySubType { get; set; }
        public object TargetState { get; set; }
        public EntityType ParentType { set { ParentTypeNullable = value; } }
        public EntityType? ParentTypeNullable { get; private set; }
        public EntityType DetailType { set { DetailTypeNullable = value; } }
        public EntityType? DetailTypeNullable { get; private set; }


        public void Apply(UserActivity entity)
        {
            entity.ActivityType = ActivityType;
            entity.TargetType = TargetType;
            entity.ApplicationType = ApplicationType;

            if (ActivitySubType != null)
                entity.ActivitySubType = ActivitySubType.ToString();

            if (TargetState != null)
                entity.TargetState = TargetState.ToString();

            if (ParentTypeNullable.HasValue)
                entity.ParentType = ParentTypeNullable.Value;

            if (DetailTypeNullable.HasValue)
                entity.DetailType = DetailTypeNullable.Value;
        }
    }
}