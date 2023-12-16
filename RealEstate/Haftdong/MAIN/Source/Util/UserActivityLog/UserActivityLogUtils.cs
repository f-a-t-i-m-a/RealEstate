using System;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    public static class UserActivityLogUtils
    {
        public static UserActivityOwinEntry CurrentEntry => OwinRequestScopeContext.Current.GetUserActivityEntry();

        public static void Set(
            this UserActivity activity, 
            EntityType? targetType = null, ObjectId? targetId = null, string targetState = null,
            UserActivityType? activityType = null, string activitySubType = null, 
            EntityType? parentType = null, ObjectId? parentId = null,
            EntityType? detailType = null, ObjectId? detailId = null, string comment= null)
        {
            if (targetType.HasValue)
                activity.TargetType = targetType.Value;

            if (targetId.HasValue)
                activity.TargetID = targetId;

            if (!targetState.IsNullOrWhitespace())
                activity.TargetState = targetState;

            if (activityType.HasValue)
                activity.ActivityType = activityType.Value;

            if (!activitySubType.IsNullOrWhitespace())
                activity.ActivitySubType = activitySubType;

            if (parentType.HasValue)
                activity.ParentType = parentType;

            if (parentId.HasValue)
                activity.ParentID = parentId;

            if (detailType.HasValue)
                activity.DetailType = detailType;

            if (detailId.HasValue)
                activity.DetailID = detailId;

            if (!comment.IsNullOrWhitespace())
                activity.Comment = comment;
        }

        public static void SetMainActivity(
            ApplicationType? applicationType = null,
            EntityType? targetType = null, ObjectId? targetId = null, string targetState = null,
            UserActivityType? activityType = null, string activitySubType = null, 
            EntityType? parentType = null, ObjectId? parentId = null,
            EntityType? detailType = null, ObjectId? detailId = null, string comment=null)
        {
            var entry = CurrentEntry;
            if (entry == null)
                return;

            entry.EnsureMainActivity();
            CurrentEntry.MainActivity.IsMainActivity = true;
            CurrentEntry.MainActivity.Set(targetType, targetId, targetState, activityType, activitySubType, parentType, parentId, detailType, detailId, comment);
        }

        public static void ReportAdditionalActivity(UserActivity activity)
        {
            var entry = CurrentEntry;
            if (entry == null)
                 throw new InvalidOperationException("There is no current UserActivity entry in the context");

            activity.IsMainActivity = false;
            entry.ReportAdditionalActivity(activity);
        }
    }
}