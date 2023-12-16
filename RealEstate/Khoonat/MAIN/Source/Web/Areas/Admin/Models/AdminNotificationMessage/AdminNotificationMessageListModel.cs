using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminNotificationMessage
{
    public class AdminNotificationMessageListModel
    {
        public PagedList<NotificationMessage> NotificationMessages { get; set; }
        public string Page { get; set; }

        public long? TargetUserIDFilter { get; set; }
        public NotificationReason? ReasonFilter { get; set; }
        public NotificationSeverity? SeverityFilter { get; set; }
        public NotificationSourceEntityType? SourceEntityTypeFilter { get; set; }
        public long? SourceEntityIDFilter { get; set; }
        public bool? SeenTimeFilter { get; set; }
        public bool? AddressedTimeFilter { get; set; }
    }
}