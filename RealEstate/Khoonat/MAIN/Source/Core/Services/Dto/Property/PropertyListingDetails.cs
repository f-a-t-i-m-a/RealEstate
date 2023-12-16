using System;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services.Dto.Property
{
	public class PropertyListingDetails
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public DateTime? DeleteDate { get; set; }
		public bool? Approved { get; set; }

		//
		// Listing type

		public PropertyType PropertyType { get; set; }
		public IntentionOfOwner IntentionOfOwner { get; set; }

		//
		// Location

        public long? VicinityID { get; set; }
		public DbGeography GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

		//
		// Related entities

		public Estate Estate { get; set; }
		public Building Building { get; set; }
		public Unit Unit { get; set; }
		public SaleConditions SaleConditions { get; set; }
		public RentConditions RentConditions { get; set; }

        public long? ContactInfoID { get; set; }
		public PropertyListingContactInfo ContactInfo { get; set; }

        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }

		//
		// Visit and Coordination

		public string AppropriateVisitTimes { get; set; }
		public string InappropriateVisitTimes { get; set; }
		public bool ShouldCoordinateBeforeVisit { get; set; }
		public string HowToCoordinateBeforeVisit { get; set; }

		//
		// Property status and Delivery

		public PropertyStatus? PropertyStatus { get; set; }
		public DateTime? RenterContractEndDate { get; set; }
		public DeliveryDateSpecificationType? DeliveryDateSpecificationType { get; set; }
		public DateTime? AbsoluteDeliveryDate { get; set; }
		public int? DeliveryDaysAfterContract { get; set; }

		//
		// Listing Status

		public DateTime CreationDate { get; set; }
		public DateTime ModificationDate { get; set; }
		public DateTime? PublishDate { get; set; }
		public DateTime? PublishEndDate { get; set; }

		//
		// Owner

		public string EditPassword { get; set; }
		public long? CreatorSessionID { get; set; }
		public long? CreatorUserID { get; set; }
		public long? OwnerUserID { get; set; }
		public string OwnerUserLoginName { get; set; }
		public string OwnerUserDisplayName { get; set; }

		//
		// Statistics

		public long NumberOfVisits { get; set; }
        public long NumberOfContactInfoRetrievals { get; set; }
        public long NumberOfSearches { get; set; }
		public int NumberOfPhotos { get; set; }
		public long? CoverPhotoId { get; set; }
		public bool IsFavoritedByUser { get; set; }
		public int TimesFavorited { get; set; }

		#region Public helper methods

		public static PropertyListingDetails MakeDetails(PropertyListing l)
		{
			return new PropertyListingDetails
			{
				ID = l.ID,
				Code = l.Code,
				DeleteDate = l.DeleteDate,
				Approved = l.Approved,
				PropertyType = l.PropertyType,
				IntentionOfOwner = l.IntentionOfOwner,
                VicinityID = l.VicinityID,
				GeographicLocation = l.GeographicLocation,
				GeographicLocationType = l.GeographicLocationType,
				Estate = l.Estate,
				Building = l.Building,
				Unit = l.Unit,
				SaleConditions = l.SaleConditions,
				RentConditions = l.RentConditions,
				ContactInfoID = l.ContactInfoID,
				ContactInfo = l.ContactInfo,
				AppropriateVisitTimes = l.AppropriateVisitTimes,
				InappropriateVisitTimes = l.InappropriateVisitTimes,
				ShouldCoordinateBeforeVisit = l.ShouldCoordinateBeforeVisit,
				HowToCoordinateBeforeVisit = l.HowToCoordinateBeforeVisit,
				PropertyStatus = l.PropertyStatus,
				RenterContractEndDate = l.RenterContractEndDate,
				DeliveryDateSpecificationType = l.DeliveryDateSpecificationType,
				AbsoluteDeliveryDate = l.AbsoluteDeliveryDate,
				DeliveryDaysAfterContract = l.DeliveryDaysAfterContract,
				CreationDate = l.CreationDate,
				ModificationDate = l.ModificationDate,
				PublishDate = l.PublishDate,
				PublishEndDate = l.PublishEndDate,
				EditPassword = l.EditPassword,
				CreatorSessionID = l.CreatorSessionID,
				CreatorUserID = l.CreatorUserID,
				OwnerUserID = l.OwnerUserID,
				OwnerUserLoginName = l.OwnerUser != null ? l.OwnerUser.LoginName : null,
				OwnerUserDisplayName = l.OwnerUser != null ? l.OwnerUser.DisplayName : null,
				NumberOfVisits = l.NumberOfVisits,
                NumberOfContactInfoRetrievals = l.NumberOfContactInfoRetrievals,
				NumberOfSearches = l.NumberOfSearches,
				NumberOfPhotos = l.Photos != null ? l.Photos.Count(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value) : 0,
				CoverPhotoId = l.Photos != null ? l.Photos.Where(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value).OrderByDescending(plp => plp.Order).Select(plp => (long?)plp.ID).FirstOrDefault() : null,
				IsFavoritedByUser = ServiceContext.Principal.CoreIdentity.UserId.HasValue && l.FavoritedBy != null && l.FavoritedBy.Any(f => f.UserID == ServiceContext.Principal.CoreIdentity.UserId.Value),
				TimesFavorited = l.FavoritedBy != null ? l.FavoritedBy.Count() : 0,
                IsAgencyActivityAllowed = l.IsAgencyActivityAllowed,
                IsAgencyListing = l.IsAgencyListing
			};
		}

		public static IQueryable<PropertyListingDetails> MakeDetails(IQueryable<PropertyListing> query, bool includeEstate = false, bool includeBuilding = false, bool includeUnit = false, bool includeSaleAndRentConditions = false, bool includeContactInfo = false)
		{
			var userId = ServiceContext.Principal.CoreIdentity.UserId;

			Expression<Func<PropertyListing, PropertyListingDetails>> detailsExpression = l => new PropertyListingDetails
			{
				ID = l.ID,
				Code = l.Code,
				DeleteDate = l.DeleteDate,
				Approved = l.Approved,
				PropertyType = l.PropertyType,
				IntentionOfOwner = l.IntentionOfOwner,
                VicinityID = l.VicinityID,
				GeographicLocation = l.GeographicLocation,
				GeographicLocationType = l.GeographicLocationType,
				Estate = l.Estate,
				Building = l.Building,
				Unit = l.Unit,
				SaleConditions = l.SaleConditions,
				RentConditions = l.RentConditions,
                ContactInfoID = l.ContactInfoID,
                ContactInfo = l.ContactInfo,
				AppropriateVisitTimes = l.AppropriateVisitTimes,
				InappropriateVisitTimes = l.InappropriateVisitTimes,
				ShouldCoordinateBeforeVisit = l.ShouldCoordinateBeforeVisit,
				HowToCoordinateBeforeVisit = l.HowToCoordinateBeforeVisit,
				PropertyStatus = l.PropertyStatus,
				RenterContractEndDate = l.RenterContractEndDate,
				DeliveryDateSpecificationType = l.DeliveryDateSpecificationType,
				AbsoluteDeliveryDate = l.AbsoluteDeliveryDate,
				DeliveryDaysAfterContract = l.DeliveryDaysAfterContract,
				CreationDate = l.CreationDate,
				ModificationDate = l.ModificationDate,
				PublishDate = l.PublishDate,
				PublishEndDate = l.PublishEndDate,
				EditPassword = l.EditPassword,
				CreatorSessionID = l.CreatorSessionID,
				CreatorUserID = l.CreatorUserID,
				OwnerUserID = l.OwnerUserID,
				OwnerUserLoginName = l.OwnerUser.LoginName,
				OwnerUserDisplayName = l.OwnerUser.DisplayName,
				NumberOfVisits = l.NumberOfVisits,
                NumberOfContactInfoRetrievals = l.NumberOfContactInfoRetrievals,
				NumberOfSearches = l.NumberOfSearches,
				NumberOfPhotos = l.Photos.Count(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value),
				CoverPhotoId = l.Photos.Where(plp => !plp.DeleteTime.HasValue && plp.Approved.HasValue && plp.Approved.Value).OrderByDescending(plp => plp.Order).Select(plp => (long?)plp.ID).FirstOrDefault(),
				IsFavoritedByUser = userId.HasValue && l.FavoritedBy.Any(f => f.UserID == userId.Value),
				TimesFavorited = l.FavoritedBy.Count(),
                IsAgencyActivityAllowed = l.IsAgencyActivityAllowed,
                IsAgencyListing = l.IsAgencyListing
			};
			
			var memberInitExpression = (MemberInitExpression)detailsExpression.Body;
			var bindings = memberInitExpression.Bindings.ToList();

			if (!includeEstate)
				bindings.Remove(bindings.Single(b => b.Member.Name == "Estate"));
			if (!includeBuilding)
				bindings.Remove(bindings.Single(b => b.Member.Name == "Building"));
			if (!includeUnit)
				bindings.Remove(bindings.Single(b => b.Member.Name == "Unit"));
			if (!includeSaleAndRentConditions)
			{
				bindings.Remove(bindings.Single(b => b.Member.Name == "SaleConditions"));
				bindings.Remove(bindings.Single(b => b.Member.Name == "RentConditions"));
			}
			if (!includeContactInfo)
				bindings.Remove(bindings.Single(b => b.Member.Name == "ContactInfo"));

			var lambda = Expression.Lambda<Func<PropertyListing, PropertyListingDetails>>(
				Expression.MemberInit(memberInitExpression.NewExpression, bindings),
				detailsExpression.TailCall,
				detailsExpression.Parameters);
			return query.Select(lambda);
		}

		#endregion

	}
}