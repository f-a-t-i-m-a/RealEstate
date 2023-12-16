using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    [AutoMapperConfig]
    public class UpdatePropertyInput
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public PropertyType PropertyType { get; set; }
        public UsageType? UsageType { get; set; }
        public string Address { get; set; }
        public VicinityReference Vicinity { get; set; }
        public LatLng GeographicLocation { get; set; }
        public decimal? EstateArea { get; set; }
        public EstateDirection? EstateDirection { get; set; }
        public decimal? PassageEdgeLength { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }
        public string LicencePlate { get; set; }
        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }
        public decimal? UnitArea { get; set; }
        public decimal? OfficeArea { get; set; }
        public decimal? CeilingHeight { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }
        public string AdditionalSpecialFeatures { get; set; }
        public int? NumberOfMasterBedrooms { get; set; }
        public KitchenCabinetType? KitchenCabinetType { get; set; }
        public DaylightDirection? MainDaylightDirection { get; set; }
        public FloorCoverType? LivingRoomFloor { get; set; }
        public BuildingFaceType? FaceType { get; set; }
        public bool? ConversionWarning { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsDuplex { get; set; }
        public bool? HasIranianLavatory { get; set; }
        public bool? HasForeignLavatory { get; set; }
        public bool? HasPrivatePatio { get; set; }
        public bool? HasBeenReconstructed { get; set; }
        public bool? HasElevator { get; set; }
        public bool? HasGatheringHall { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public bool? HasSwimmingPool { get; set; }
        public bool? HasSauna { get; set; }
        public bool? HasJacuzzi { get; set; }
        public NewCustomerInput Owner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdatePropertyInput, Property>()
                .IgnoreUnmappedProperties();
        }
    }
}