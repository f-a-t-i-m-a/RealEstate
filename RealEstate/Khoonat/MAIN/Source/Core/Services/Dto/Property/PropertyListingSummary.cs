using System;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services.Dto.Property
{
	public class PropertyListingSummary
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public DateTime? DeleteDate { get; set; }
		public bool? Approved { get; set; }

		public PropertyType PropertyType { get; set; }
		public IntentionOfOwner IntentionOfOwner { get; set; }
        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }

        public long? VicinityID { get; set; }
		public DbGeography GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

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

		#region Public helper methods

		public static PropertyListingSummary Summarize(PropertyListingDetails l)
		{
			return new PropertyListingSummary
			{
				ID = l.ID,
				Code = l.Code,
				DeleteDate = l.DeleteDate,
				Approved = l.Approved,
				PropertyType = l.PropertyType,
				IntentionOfOwner = l.IntentionOfOwner,
                IsAgencyActivityAllowed = l.IsAgencyActivityAllowed,
                IsAgencyListing = l.IsAgencyListing,
                VicinityID = l.VicinityID,
				GeographicLocation = l.GeographicLocation,
				GeographicLocationType = l.GeographicLocationType,
				EstateDirection = l.Estate?.Direction,
				EstateSurfaceType = l.Estate?.SurfaceType,
				TotalNumberOfUnitsInBuilding = l.Building?.TotalNumberOfUnits,
				TotalNumberOfFloorsInBuilding = l.Building?.NumberFloorsAboveGround,
				UsageType = l.Unit?.UsageType,
				NumberOfRooms = l.Unit?.NumberOfRooms,
				FloorNumber = l.Unit?.FloorNumber,
                HasElevator = l.Building?.HasElevator,
                NumberOfParkings = l.Unit?.NumberOfParkings,
                StorageRoomArea = l.Unit?.StorageRoomArea,
                HasBeenReconstructed = l.Unit?.HasBeenReconstructed,
				EstateArea = l.Estate?.Area,
				UnitArea = l.Unit?.Area,
				Price = l.SaleConditions?.Price,
				PricePerEstateArea = l.SaleConditions?.PricePerEstateArea,
				PricePerUnitArea = l.SaleConditions?.PricePerUnitArea,
				HasTransferableLoan = l.SaleConditions?.HasTransferableLoan,
				Mortgage = l.RentConditions?.Mortgage,
				Rent = l.RentConditions?.Rent,
				MortgageAndRentConvertible = l.RentConditions?.MortgageAndRentConvertible,
				PublishDate = l.PublishDate,
				PublishEndDate = l.PublishEndDate,
				OwnerUserID = l.OwnerUserID,
				CreationDate = l.CreationDate,
				ModificationDate = l.ModificationDate,
				Visits = l.NumberOfVisits,
                ContactInfoRetrievals = l.NumberOfContactInfoRetrievals,
				Searches = l.NumberOfSearches,
				NumberOfPhotos = l.NumberOfPhotos,
				CoverPhotoId = l.CoverPhotoId,
				IsFavoritedByUser = l.IsFavoritedByUser,
				TimesFavorited = l.TimesFavorited
			};
		}

		public static IQueryable<PropertyListingSummary> Summarize(IQueryable<PropertyListing> query)
		{
			var userId = ServiceContext.Principal.CoreIdentity.UserId;

			Expression<Func<PropertyListing, PropertyListingSummary>> summaryExpression = l => new PropertyListingSummary
			{
				ID = l.ID,
				Code = l.Code,
				DeleteDate = l.DeleteDate,
				Approved = l.Approved,
				PropertyType = l.PropertyType,
				IntentionOfOwner = l.IntentionOfOwner,
                IsAgencyActivityAllowed = l.IsAgencyActivityAllowed,
                IsAgencyListing = l.IsAgencyListing,
                VicinityID = l.VicinityID,
				GeographicLocation = l.GeographicLocation,
				GeographicLocationType = l.GeographicLocationType,
				EstateDirection = l.Estate.Direction,
				EstateSurfaceType = l.Estate.SurfaceType,
				TotalNumberOfUnitsInBuilding = l.Building.TotalNumberOfUnits,
				TotalNumberOfFloorsInBuilding = l.Building.NumberFloorsAboveGround,
				UsageType = l.Unit.UsageType,
				NumberOfRooms = l.Unit.NumberOfRooms,
				FloorNumber = l.Unit.FloorNumber,
                HasElevator = l.Building.HasElevator,
                NumberOfParkings = l.Unit.NumberOfParkings,
                StorageRoomArea = l.Unit.StorageRoomArea,
                HasBeenReconstructed = l.Unit.HasBeenReconstructed,
				EstateArea = l.Estate.Area,
				UnitArea = l.Unit.Area,
				Price = l.SaleConditions.Price,
				PricePerEstateArea = l.SaleConditions.PricePerEstateArea,
				PricePerUnitArea = l.SaleConditions.PricePerUnitArea,
				HasTransferableLoan = l.SaleConditions.HasTransferableLoan,
				Mortgage = l.RentConditions.Mortgage,
				Rent = l.RentConditions.Rent,
				MortgageAndRentConvertible = l.RentConditions.MortgageAndRentConvertible,
				PublishDate = l.PublishDate,
				PublishEndDate = l.PublishEndDate,
				OwnerUserID = l.OwnerUserID,
				CreationDate = l.CreationDate,
				ModificationDate = l.ModificationDate,
				Visits = l.NumberOfVisits,
                ContactInfoRetrievals = l.NumberOfContactInfoRetrievals,
				Searches = l.NumberOfSearches,
				NumberOfPhotos = l.Photos.Count(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value),
				CoverPhotoId = l.Photos.Where(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value).OrderByDescending(plp => plp.Order).Select(plp => (long?)plp.ID).FirstOrDefault(),
				IsFavoritedByUser = userId.HasValue && l.FavoritedBy.Any(f => f.UserID == userId.Value),
				TimesFavorited = l.FavoritedBy.Count()
			};

			return query.Select(summaryExpression);
		}

		#endregion
	}
}