using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyListing : IIndexedEntity, IEntityContentContainer<PropertyListingContent>
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public DateTime? DeleteDate { get; set; }
		public bool? Approved { get; set; }
		public DateTime? IndexedTime { get; set; }

		//
		// Listing type

		public PropertyType PropertyType { get; set; }
		public IntentionOfOwner IntentionOfOwner { get; set; }

        public bool IsAgencyListing { get; set; }
        public bool IsAgencyActivityAllowed { get; set; }

		//
		// Content

		public string ContentString { get; set; }
		public PropertyListingContent Content { get; set; }

		//
		// Redundant properties

        public long? VicinityID { get; set; }
        public Vicinity Vicinity { get; set; }
        public string VicinityHierarchyString { get; set; }

		public DbGeography GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

		//
		// Related entities

		public long? EstateID { get; set; }
		public Estate Estate { get; set; }

		public long? BuildingID { get; set; }
		public Building Building { get; set; }

		public long? UnitID { get; set; }
		public Unit Unit { get; set; }

		public long? SaleConditionsID { get; set; }
		public SaleConditions SaleConditions { get; set; }

		public long? RentConditionsID { get; set; }
		public RentConditions RentConditions { get; set; }

		public long? ContactInfoID { get; set; }
		public PropertyListingContactInfo ContactInfo { get; set; }

        public ICollection<SponsoredPropertyListing> Sponsorships { get; set; }

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
		public HttpSession CreatorSession { get; set; }
		public long? CreatorUserID { get; set; }
		public User CreatorUser { get; set; }
		public long? OwnerUserID { get; set; }
		public User OwnerUser { get; set; }

		//
		// Statistics

		public long NumberOfVisits { get; set; }
		public long NumberOfContactInfoRetrievals { get; set; }
		public long NumberOfSearches { get; set; }

		//
		// Sort columns

		public decimal? NormalizedPrice { get; set; }
		public decimal? NormalizedPricePerEstateArea { get; set; }
		public decimal? NormalizedPricePerUnitArea { get; set; }

		//
		// Relationships to other non-major entities

		public ICollection<PropertyListingPhoto> Photos { get; set; }
		public ICollection<UserFavoritePropertyListing> FavoritedBy { get; set; }
	}
}