using System;
using JahanJooy.RealEstateAgency.Domain.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    public class PasswordResetRequest
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public ApplicationUserReference User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string PasswordResetToken { get; set; }
        public ContactInfo ContactMethod { get; set; }

    }
}