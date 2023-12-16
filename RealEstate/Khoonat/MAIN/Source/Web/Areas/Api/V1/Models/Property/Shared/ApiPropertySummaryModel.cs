using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
    public class ApiPropertySummaryModel
    {
        public long ID { get; set; }
        public long Code { get; set; }
        public PropertyType PropertyType { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public ApiGeoPoint GeographicLocation { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationType { get; set; }
        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }

        public ApiOutputVicinityModel Vicinity { get; set; }

        public EstateDirection? EstateDirection { get; set; }
        public EstateSurfaceType? EstateSurfaceType { get; set; }
        public int? TotalNumberOfUnitsInBuilding { get; set; }
        public int? TotalNumberOfFloorsInBuilding { get; set; }
        public UnitUsageType? UsageType { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? FloorNumber { get; set; }

        public bool? HasElevator { get; set; }
        public int? NumberOfParkings { get; set; }
        public decimal? StorageRoomArea { get; set; }
        public bool? HasBeenReconstructed { get; set; }


        public decimal? EstateArea { get; set; }
        public decimal? UnitArea { get; set; }

        public decimal? Price { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool? HasTransferableLoan { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool? MortgageAndRentConvertible { get; set; }

        public DateTime? PublishDate { get; set; }
        public DateTime? PublishEndDate { get; set; }
        public long? OwnerUserID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }

        public long Visits { get; set; }
        public long ContactInfoRetrievals { get; set; }
        public long Searches { get; set; }
        public int NumberOfPhotos { get; set; }
        public long? CoverPhotoId { get; set; }
        public bool IsFavoritedByUser { get; set; }
        public int TimesFavorited { get; set; }

        public ApiFileContentModel CoverPhotoFile { get; set; }

        public static void ConfigureMapper()
        {
            Mapper.CreateMap<PropertyListingSummary, ApiPropertySummaryModel>()
                .Ignore(p => p.GeographicLocation)
                .Ignore(p => p.Vicinity)
                .IgnoreUnmappedProperties();
        }
    }
}