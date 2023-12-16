using System;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Contract
{
    public class Contract
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public DateTime ContractDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Portion { get; set; }
        public string District { get; set; }
        public string RegistrationZone { get; set; }
        public string OwnershipEvidenceSerialNumber { get; set; }
        public string NotaryPublicPageNumber { get; set; }
        public string NotaryPublic { get; set; }
        public string PublicSpace { get; set; }
        public string Description { get; set; }

        public CustomerReference SellerReference { get; set; }
        public CustomerReference BuyerReference { get; set; }
        public PropertyReference PropertyReference { get; set; }
        public RequestReference RequestReference { get; set; }
        public SupplyReference SupplyReference { get; set; }

        public int? TrackingID { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public decimal? EstateArea { get; set; }
        public decimal? UnitArea { get; set; }

        public ContractState State { get; set; }


        public DateTime? LastIndexingTime { get; set; }

        public DateTime CreationTime { get; set; }
        public ObjectId CreatedByID { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }
    }
}