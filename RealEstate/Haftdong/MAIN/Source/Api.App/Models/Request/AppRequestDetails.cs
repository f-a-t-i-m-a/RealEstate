using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class AppRequestDetails: AppRequestSummary
    {
       public bool IsArchived { get; set; }

        #region Estate properties

        public decimal? EstateArea { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }
        public List<AppVicinitySummary> SelectedVicinities { get; set; }


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

        #region Rent price

        public bool MortgageAndRentConvertible { get; set; }

        #endregion


        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Request.Request, AppRequestDetails>()
                .Ignore(r => r.Vicinities)
                .IgnoreUnmappedProperties()
                .IncludeBase<Domain.Request.Request, AppRequestSummary>();

            Mapper.CreateMap<RequestDetails, AppRequestDetails>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<RequestReference, AppRequestDetails>()
            .IgnoreUnmappedProperties();
        }
    }
}