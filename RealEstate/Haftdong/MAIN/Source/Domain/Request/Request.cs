using System;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Domain.Request
{
    public class Request
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? UsageType { get; set; }
        public RequestState State { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPublic { get; set; }
        public SourceType SourceType { get; set; }
        public string Description { get; set; }

        #region Location properties
        public ObjectId[] Vicinities { get; set; }
        public SupplyReference Supply { get; set; }

        #endregion


        #region Estate properties

        public decimal? EstateArea { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion


        #region Building properties

        public int? BuildingAgeYears { get; set; }
        public int? TotalNumberOfUnits { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        #endregion


        #region Unit properties

        public decimal? UnitArea { get; set; }
        public decimal? OfficeArea { get; set; }
        public decimal? CeilingHeight { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }

        #endregion


        #region Extra unit properties

        public bool? IsDublex { get; set; }
        public bool? HasBeenReconstructed { get; set; }
        public bool? HasIranianLavatory { get; set; }
        public bool? HasForeignLavatory { get; set; }
        public bool? HasPrivatePatio { get; set; }
        public bool? HasMasterBedroom { get; set; }

        #endregion


        #region Extra building properties

        public bool? HasElevator { get; set; }
        public bool? HasGatheringHall { get; set; }
        public bool? HasSwimmingPool { get; set; }
        public bool? HasStorageRoom { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public bool? HasSauna { get; set; }
        public bool? HasJacuzzi { get; set; }

        #endregion

        #region Buy price

        public decimal? TotalPrice { get; set; }

        #endregion


        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }

        #endregion

        #region Owner Contacts

        public CustomerReference Owner { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public ContactMethodCollection OwnerContact { get; set; }
        public ContactMethodCollection AgencyContact { get; set; }

        #endregion

        #region Indexing

        public DateTime? LastIndexingTime { get; set; }

        #endregion

        public DateTime CreationTime { get; set; }
        public ObjectId CreatedByID { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }

    }
}