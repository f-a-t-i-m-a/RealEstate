using JahanJooy.RealEstateAgency.Domain.Users;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Domain.Base
{
    public abstract class ContactInfo
    {
        public ObjectId ID { get; set; }

        public string Value { get; set; }
        public string NormalizedValue { get; set; }

        public bool IsVerifiable { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public UserContactMethodVerification UserContactMethodVerification { get; set; }
    }
}