using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    [AutoMapperConfig]
    public class NewPropertyInput
    {
        #region Property
        
        public PropertyType PropertyType { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }

        #region Location properties

        public string Address { get; set; }
        public string LicencePlate { get; set; }
        public ObjectId VicinityID { get; set; }
        public LatLng GeographicLocation { get; set; }

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

        #region Sales price

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        #endregion

        public NewCustomerInput Owner { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public NewContactMethodCollectionInput ContactInfos { get; set; }

        #endregion

        #region Request

        public string SwapAdditionalComments { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public long[] PropertyTypes { get; set; }
        public UsageType? RequestUsageType { get; set; }
        public DateTime? ExpirationTime { get; set; }

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

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewPropertyInput, Property>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<NewPropertyInput, Supply>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<NewPropertyInput, Request>()
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
