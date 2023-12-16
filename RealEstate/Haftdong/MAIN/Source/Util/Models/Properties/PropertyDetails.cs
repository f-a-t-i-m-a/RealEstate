using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    [AutoMapperConfig]
    public class PropertyDetails : PropertySummary
    {
        public List<PhotoInfoSummary> Photos { get; set; }
        public string LicencePlate { get; set; }
        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }
        public PropertyStatus? PropertyStatus { get; set; }
        public EstateDirection? EstateDirection { get; set; }
        public decimal? PassageEdgeLength { get; set; }
        public EstateVoucherType? EstateVoucherType { get; set; }
        public string ExternalDetails { get; set; }

        public int? TotalNumberOfUnits { get; set; }
        public int? BuildingAgeYears { get; set; }
        public int? NumberOfUnitsPerFloor { get; set; }
        public int? TotalNumberOfFloors { get; set; }

        public decimal? OfficeArea { get; set; }
        public decimal? CeilingHeight { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public int? NumberOfParkings { get; set; }
        public int? UnitFloorNumber { get; set; }
        public string AdditionalSpecialFeatures { get; set; }

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

        public bool? HasElevator { get; set; }
        public bool? HasGatheringHall { get; set; }
        public bool? HasAutomaticParkingDoor { get; set; }
        public bool? HasVideoEyePhone { get; set; }
        public bool? HasSwimmingPool { get; set; }
        public bool? HasSauna { get; set; }
        public bool? HasJacuzzi { get; set; }
        
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }
        
        public bool MortgageAndRentConvertible { get; set; }
        public string AdditionalRentalComments { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        public Guid CorrelationID { get; set; }
        public bool IsMasterBuiling { get; set; }
        public List<PropertyDetails> RelatedProperties { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Property, PropertyDetails>()
                .IncludeBase<Property, PropertySummary>()
                .IgnoreUnmappedProperties();
        }
    }
}