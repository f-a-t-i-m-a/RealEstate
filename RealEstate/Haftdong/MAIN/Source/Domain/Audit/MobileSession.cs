using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Audit
{
    public class MobileSession
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string SessionID { get; set; }
        public DateTime CreationTime { get; set; }
        public ObjectId UserId { get; set; }

        //Mobile Subscriber
        public string PhoneSubscriberID { get; set; }
        public string PhoneOperator { get; set; }

        //Mobile Device
        public string PhoneSerialNumber { get; set; }

        //User Agent
        public string UserAgentString { get; set; }

    }
}