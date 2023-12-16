using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminOutgoingSms
{
    public class AdminOutgoingSmsListModel
    {
        public PagedList<OutgoingSmsMessage> Messages { get; set; }
        public string Page { get; set; }

        public long? OutgoingSmsIdFilter { get; set; }
        public bool ApplyTargetUserIdFilter { get; set; }
        public long? TargetUserIdFilter { get; set; }

        public NotificationReason? ReasonFilter { get; set; }
        public OutgoingSmsMessageState? StateFilter { get; set; }
        public NotificationSourceEntityType? SourceEntityTypeFilter { get; set; }
        public long? SourceEntityIdFilter { get; set; }
        public int? RetryIndexFilter { get; set; }

        public string TargetNumberFilter { get; set; }
        public string MessageTextFilter { get; set; }
        public string SenderNumberFilter { get; set; }

        public int? ErrorCodeFilter { get; set; }
        public int? LastDeliveryCodeFilter { get; set; }
        public string OperatorAssignedIdFilter { get; set; }
    }
}