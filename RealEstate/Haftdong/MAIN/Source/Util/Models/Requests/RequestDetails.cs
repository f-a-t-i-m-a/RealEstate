using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [AutoMapperConfig]
    public class RequestDetails: RequestSummary
    {
       public bool IsArchived { get; set; }

        #region Estate properties

        public decimal? EstateArea { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }
        public string Description { get; set; }
        public List<VicinitySummary> SelectedVicinities { get; set; }
        public SupplySummary Supply { get; set; }

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

        public bool? ContactToOwner { get; set; }
        public bool? ContactToAgency { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Request, RequestDetails>()
                .ForMember(s => s.ContactToOwner, opt => opt.MapFrom(su => su.OwnerCanBeContacted == true))
                .ForMember(s => s.ContactToAgency, opt => opt.MapFrom(su => su.OwnerCanBeContacted == false))
                .IgnoreUnmappedProperties()
                .Ignore(r => r.Vicinities)
                .IncludeBase<Request, RequestSummary>();

            Mapper.CreateMap<RequestReference, RequestDetails>()
            .IgnoreUnmappedProperties();
        }
    }
}