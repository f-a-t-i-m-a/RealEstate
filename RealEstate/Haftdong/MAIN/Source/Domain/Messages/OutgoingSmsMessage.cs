using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Domain.Messages
{
    public class OutgoingSmsMessage
    {
        public ObjectId ID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? TransmissionDate { get; set; }
        public ObjectId? TargetUserID { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool AllowTransmissionOnAnyTimeOfDay { get; set; }

        public NotificationReason Reason { get; set; }
        public OutgoingSmsMessageState State { get; set; }
        public DateTime StateDate { get; set; }

        public NotificationSourceEntityType SourceEntityType { get; set; }
        public ObjectId SourceEntityID { get; set; }
        public NotificationMessage Notification { get; set; }
        public ObjectId? NotificationID { get; set; }

        public int RetryIndex { get; set; }
        public ObjectId? RetryForMessageID { get; set; }
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
