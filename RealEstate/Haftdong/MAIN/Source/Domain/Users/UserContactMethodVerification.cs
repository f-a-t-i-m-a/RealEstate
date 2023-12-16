using System;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    public class UserContactMethodVerification
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpirationTime { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CompletionTime { get; set; }
        public string VerificationSecret { get; set; }
    }
}
