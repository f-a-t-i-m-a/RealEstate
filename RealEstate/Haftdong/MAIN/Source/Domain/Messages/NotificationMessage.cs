using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JahanJooy.Common.Util.DomainModel;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Domain.Messages
{
    public class NotificationMessage : ICreationTime
    {
        public ObjectId ID { get; set; }
        public DateTime CreationTime { get; set; }
        public ObjectId TargetUserID { get; set; }

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
