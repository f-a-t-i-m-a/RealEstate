using System;
using System.Collections.Generic;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Domain.Messages
{
    public class NotificationMessage : ICreationTime
    {
		public long ID { get; set; }
		public DateTime CreationTime { get; set; }
		public long TargetUserID { get; set; }
		public User TargetUser { get; set; }

		public string Text { get; set; }
		public NotificationReason Reason { get; set; }
		public NotificationSeverity Severity { get; set; }
		public NotificationSourceEntityType? SourceEntityType { get; set; }
		public long? SourceEntityID { get; set; }

		public DateTime? SeenTime { get; set; }
		public DateTime? AddressedTime { get; set; }
		public DateTime? NextMessageTransmissionDue { get; set; }

		public ICollection<OutgoingSmsMessage> SmsMessages { get; set; }
    }
}