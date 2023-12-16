using System;
using JahanJooy.RealEstateAgency.Domain.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Customers
{
    public class Customer
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string DisplayName { get; set; }
        public int? Age { get; set; }
        public bool? IsMarried { get; set; }
        public ContactMethodCollection Contact { get; set; }
        public string Description { get; set; }
        public ObjectId UserID { get; set; }

        public int RequestCount { get; set; }
        public int PropertyCount { get; set; }

        public DateTime? LastIndexingTime { get; set; }
        public DateTime? LastVisitTime { get; set; }

        public string NameOfFather { get; set; }
        public int? Identification { get; set; }
        public string IssuedIn { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public CustomerReference Deputy { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreationTime { get; set; }
        public ObjectId CreatedByID { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }
    }
}
