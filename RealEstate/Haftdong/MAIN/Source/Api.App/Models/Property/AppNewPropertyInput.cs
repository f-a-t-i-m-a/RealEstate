using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewPropertyInput
    {
        public PropertyType PropertyType { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public bool IsPublic { get; set; }

        #region Location properties

        public string Address { get; set; }
        public VicinityReference Vicinity { get; set; }
        public LatLng GeographicLocation { get; set; }

        #endregion

        #region Estate properties

        public decimal? PropertyEstateArea { get; set; }
        public EstateDirection? EstateDirection { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion

        #region Building properties

        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        #endregion

        #region Unit properties

        public decimal? PropertyUnitArea { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public UsageType? PropertyUsageType { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }
        public string AdditionalSpecialFeatures { get; set; }

        #endregion

        #region Extra unit properties

        public int? NumberOfMasterBedrooms { get; set; }
        public DaylightDirection? MainDaylightDirection { get; set; }

        #endregion

        #region Extra building properties

        public bool? HasElevator { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }

        #endregion

        #region Sales price

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? PropertyTotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

        public decimal? PropertyMortgage { get; set; }
        public decimal? PropertyRent { get; set; }
        public bool PropertyMortgageAndRentConvertible { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        #endregion
        #region Request

        public string SwapAdditionalComments { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? RequestUsageType { get; set; }

        #region Location properties
        public ObjectId[] Vicinities { get; set; }
        public string Description { get; set; }

        #endregion

        #region Estate properties

        public decimal? RequestEstateArea { get; set; }

        #endregion

        #region Unit properties

        public decimal? RequestUnitArea { get; set; }
        public decimal? RequestOfficeArea { get; set; }

        #endregion

        #region Sales price

        public decimal? RequestTotalPrice { get; set; }

        #endregion


        #region Rent price

        public decimal? RequestMortgage { get; set; }
        public decimal? RequestRent { get; set; }
        public bool RequestMortgageAndRentConvertible { get; set; }

        #endregion

        #endregion
        public AppNewCustomerInput Owner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewPropertyInput, Domain.Property.Property>()
                .ForMember(p => p.UsageType, opt => opt.MapFrom(pp => pp.PropertyUsageType))
                .ForMember(p => p.EstateArea, opt => opt.MapFrom(pp => pp.PropertyEstateArea))
                .ForMember(p => p.UnitArea, opt => opt.MapFrom(pp => pp.PropertyUnitArea))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<AppNewPropertyInput, Domain.Supply.Supply>()
                .ForMember(p => p.TotalPrice, opt => opt.MapFrom(pp => pp.PropertyTotalPrice))
                .ForMember(p => p.Mortgage, opt => opt.MapFrom(pp => pp.PropertyMortgage))
                .ForMember(p => p.Rent, opt => opt.MapFrom(pp => pp.PropertyRent))
                .ForMember(p => p.MortgageAndRentConvertible, opt => opt.MapFrom(pp => pp.PropertyMortgageAndRentConvertible))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<AppNewPropertyInput, Domain.Request.Request>()
                .ForMember(p => p.UsageType, opt => opt.MapFrom(pp => pp.RequestUsageType))
                .ForMember(p => p.EstateArea, opt => opt.MapFrom(pp => pp.RequestEstateArea))
                .ForMember(p => p.UnitArea, opt => opt.MapFrom(pp => pp.RequestUnitArea))
                .ForMember(p => p.OfficeArea, opt => opt.MapFrom(pp => pp.RequestOfficeArea))
                .ForMember(p => p.TotalPrice, opt => opt.MapFrom(pp => pp.RequestTotalPrice))
                .ForMember(p => p.Mortgage, opt => opt.MapFrom(pp => pp.RequestMortgage))
                .ForMember(p => p.Rent, opt => opt.MapFrom(pp => pp.RequestRent))
                .ForMember(p => p.MortgageAndRentConvertible, opt => opt.MapFrom(pp => pp.RequestMortgageAndRentConvertible))
                .IgnoreUnmappedProperties();
        }
    }
}
