using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class AppUpdatePropertyInput
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
        public EstateVoucherType? EstateVoucherType { get; set; }

        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        public decimal? UnitArea { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }
        public string AdditionalSpecialFeatures { get; set; }

        public int? NumberOfMasterBedrooms { get; set; }
        public DaylightDirection? MainDaylightDirection { get; set; }

        public bool? HasElevator { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public AppNewCustomerInput Owner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppUpdatePropertyInput, Domain.Property.Property>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<AppUpdatePropertyInput, UpdatePropertyInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
