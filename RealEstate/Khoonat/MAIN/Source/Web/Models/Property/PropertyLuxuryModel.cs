using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyLuxuryModel : PropertySectionModelBase
	{
		public PropertyLuxuryModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#region User-input

		[Numeric(ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Range")]
		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_NumberOfMasterBedrooms")]
		public int? NumberOfMasterBedrooms { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_CeilingHeight_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_CeilingHeight_Numeric")]
		[Range(2, 12, ErrorMessageResourceType = typeof(PropertyLuxuryResources), ErrorMessageResourceName = "Validation_CeilingHeight_Range")]
		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_CeilingHeight")]
		public decimal? CeilingHeight { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasAllSideView")]
		public bool? HasAllSideView { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasAllSideBalcony")]
		public bool? HasAllSideBalcony { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasPrivatePool")]
		public bool? HasPrivatePool { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasPrivateElevator")]
		public bool? HasPrivateElevator { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasInUnitParking")]
		public bool? HasInUnitParking { get; set; }

        [Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasOpenningCeiling")]
        public bool? HasOpenningCeiling { get; set; }

        [Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasMobileWall")]
        public bool? HasMobileWall { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasGardenInBalcony")]
		public bool? HasGardenInBalcony { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasRemoteControlledCurtains")]
		public bool? HasRemoteControlledCurtains { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasBuildingManagementSystem")]
		public bool? HasBuildingManagementSystem { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasPrivateJanitorUnit")]
		public bool? HasPrivateJanitorUnit { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_HasGuestSuite")]
		public bool? HasGuestSuite { get; set; }

		[Display(ResourceType = typeof(PropertyLuxuryResources), Name = "Label_AdditionalLuxuryFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AdditionalLuxuryFeatures { get; set; }

		#endregion

		#region Convert to/from domain

		public static PropertyLuxuryModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertyLuxuryModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (domain.Unit != null)
			{
				result.NumberOfMasterBedrooms = domain.Unit.NumberOfMasterBedrooms;
				result.CeilingHeight = domain.Unit.CeilingHeight;
				result.HasAllSideView = domain.Unit.HasAllSideView;
				result.HasAllSideBalcony = domain.Unit.HasAllSideBalcony;
				result.HasPrivatePool = domain.Unit.HasPrivatePool;
				result.HasPrivateElevator = domain.Unit.HasPrivateElevator;
				result.HasInUnitParking = domain.Unit.HasInUnitParking;
				result.HasOpenningCeiling = domain.Unit.HasOpenningCeiling;
				result.HasMobileWall = domain.Unit.HasMobileWall;
				result.HasGardenInBalcony = domain.Unit.HasGardenInBalcony;
				result.HasRemoteControlledCurtains = domain.Unit.HasRemoteControlledCurtains;
				result.HasBuildingManagementSystem = domain.Unit.HasBuildingManagementSystem;
				result.HasPrivateJanitorUnit = domain.Unit.HasPrivateJanitorUnit;
				result.HasGuestSuite = domain.Unit.HasGuestSuite;
				result.AdditionalLuxuryFeatures = domain.Unit.AdditionalLuxuryFeatures;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.Unit == null)
				domain.Unit = new Unit();

			domain.Unit.NumberOfMasterBedrooms = NumberOfMasterBedrooms;
			domain.Unit.CeilingHeight = CeilingHeight;
			domain.Unit.HasAllSideView = HasAllSideView;
			domain.Unit.HasAllSideBalcony = HasAllSideBalcony;
			domain.Unit.HasPrivatePool = HasPrivatePool;
			domain.Unit.HasPrivateElevator = HasPrivateElevator;
			domain.Unit.HasInUnitParking = HasInUnitParking;
			domain.Unit.HasOpenningCeiling = HasOpenningCeiling;
			domain.Unit.HasMobileWall = HasMobileWall;
			domain.Unit.HasGardenInBalcony = HasGardenInBalcony;
			domain.Unit.HasRemoteControlledCurtains = HasRemoteControlledCurtains;
			domain.Unit.HasBuildingManagementSystem = HasBuildingManagementSystem;
			domain.Unit.HasPrivateJanitorUnit = HasPrivateJanitorUnit;
			domain.Unit.HasGuestSuite = HasGuestSuite;
			domain.Unit.AdditionalLuxuryFeatures = AdditionalLuxuryFeatures;
		}

		#endregion
	}
}