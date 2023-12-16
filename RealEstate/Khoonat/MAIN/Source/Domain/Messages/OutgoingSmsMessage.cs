using System;

namespace JahanJooy.RealEstate.Domain.Messages
{
    public class OutgoingSmsMessage
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? TransmissionDate { get; set; }
        public long? TargetUserID { get; set; }
        public User TargetUser { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool AllowTransmissionOnAnyTimeOfDay { get; set; }

        public NotificationReason Reason { get; set; }
        public OutgoingSmsMessageState State { get; set; }
        public DateTime StateDate { get; set; }

        public NotificationSourceEntityType SourceEntityType { get; set; }
        public long SourceEntityID { get; set; }
		public NotificationMessage Notification { get; set; }
		public long? NotificationID { get; set; }

        public int RetryIndex { get; set; }
        public long? RetryForMessageID { get; set; }
        public OutgoingSmsMessage RetryForMessage { get; set; }

        public string TargetNumber { get; set; }
        public string MessageText { get; set; }
        public bool IsFlash { get; set; }

		public string SenderNumber { get; set; }
		public int? ErrorCode { get; set; }
        public int? LastDeliveryCode { get; set; }
        public string OperatorAssignedID { get; set; }
    }
}