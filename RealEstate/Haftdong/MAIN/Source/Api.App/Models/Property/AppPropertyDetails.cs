using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class AppPropertyDetails : AppPropertySummary
    {
        public List<AppPhotoInfoSummary> Photos { get; set; }
        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }

        public EstateDirection? EstateDirection { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }

        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

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

        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        public bool MortgageAndRentConvertible { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Property.Property, AppPropertyDetails>()
                .IncludeBase<Domain.Property.Property, AppPropertySummary>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<PropertyDetails, AppPropertyDetails>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<PropertySummary, AppPropertyDetails>()
                .IgnoreUnmappedProperties();
        }
    }
}