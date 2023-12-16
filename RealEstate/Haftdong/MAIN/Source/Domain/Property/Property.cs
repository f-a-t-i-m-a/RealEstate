using System;
using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.Domain.Property
{
    public class Property
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public PropertyType PropertyType { get; set; }
        public PropertyState State { get; set; }
        public string LicencePlate { get; set; }
        public bool IsArchived { get; set; }
        public bool? IsHidden { get; set; }
        public bool? ConversionWarning { get; set; }
        public ObjectId? CoverImageID { get; set; }
        public List<PhotoInfo> Photos { get; set; }
        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }
        public PropertyStatus? PropertyStatus { get; set; }
        public string ExternalDetails { get; set; }

        #region Owner Contacts

        public CustomerReference Owner { get; set; }

        #endregion

        #region Supplies

        public List<SupplyReference> Supplies { get; set; }

        #endregion

        #region Indexing

        public DateTime? LastIndexingTime { get; set; }

        #endregion

        public DateTime CreationTime { get; set; }
        public ObjectId? CreatedByID { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }

        #region Location properties

        public string Address { get; set; }
        public VicinityReference Vicinity { get; set; }
        public GeoJson2DCoordinates GeographicLocation { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

        #endregion

        #region Estate properties

        public decimal? EstateArea { get; set; }
        public EstateDirection? EstateDirection { get; set; }
        public decimal? PassageEdgeLength { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion

        #region Building properties

        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        #endregion

        #region Unit properties

        public decimal? UnitArea { get; set; }
        public decimal? OfficeArea { get; set; }
        public decimal? CeilingHeight { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public UsageType? UsageType { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }
        public string AdditionalSpecialFeatures { get; set; }

        #endregion

        #region Extra unit properties

        public int? NumberOfMasterBedrooms { get; set; }
        public KitchenCabinetType? KitchenCabinetType { get; set; }
        public DaylightDirection? MainDaylightDirection { get; set; }
        public FloorCoverType? LivingRoomFloor { get; set; }
        public BuildingFaceType? FaceType { get; set; }
        public bool? IsDuplex { get; set; }
        public bool? HasIranianLavatory { get; set; }
        public bool? HasForeignLavatory { get; set; }
        public bool? HasPrivatePatio { get; set; }
        public bool? HasBeenReconstructed { get; set; }

        #endregion

        #region Extra building properties

        public bool? HasElevator { get; set; }
        public bool? HasGatheringHall { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public bool? HasSwimmingPool { get; set; }
        public bool? HasSauna { get; set; }
        public bool? HasJacuzzi { get; set; }

        #endregion

        #region Tenement properties

        [BsonRepresentation(BsonType.String)]
        public Guid CorrelationID { get; set; }
        public bool IsMasterBuiling { get; set; }

        #endregion

        #region fetching source

        public string ExternalID { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime? LastFetchTime { get; set; }

        #endregion
    }
}