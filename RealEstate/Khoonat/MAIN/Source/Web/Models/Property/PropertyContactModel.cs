using System;
using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyContactModel : PropertySectionModelBase
	{
		public PropertyContactModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

        public bool IsAgencyListing { get; set; }

		#region User-input: Visiting

		[Display(ResourceType = typeof (PropertyContactResources), Name = "Label_AppropriateVisitTimes")]
		[StringLength(500, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AppropriateVisitTimes { get; set; }

		[Display(ResourceType = typeof (PropertyContactResources), Name = "Label_InappropriateVisitTimes")]
		[StringLength(500, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string InappropriateVisitTimes { get; set; }

		[Display(ResourceType = typeof (PropertyContactResources), Name = "Label_ShouldCoordinateBeforeVisit")]
		public bool ShouldCoordinateBeforeVisit { get; set; }

		[SkipValidationIfProperty("ShouldCoordinateBeforeVisit", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_HowToCoordinateBeforeVisit")]
		[StringLength(500, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string HowToCoordinateBeforeVisit { get; set; }

		#endregion

		#region User-input: Status and Delivery

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_PropertyStatus")]
		public PropertyStatus? PropertyStatus { get; set; }

		[SkipValidationIfProperty("PropertyStatus", PropertyValidationComparisonUtil.ComparisonType.NotEquals, Domain.Enums.PropertyStatus.OccupiedByRenter)]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_RenterContractEndDate_Date")]
		[DynamicDateRange(1, 1825, ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_RenterContractEndDate_Range")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_RenterContractEndDate")]
		public DateTime? RenterContractEndDate { get; set; }

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_DeliveryDateSpecificationType")]
		public DeliveryDateSpecificationType? DeliveryDateSpecificationType { get; set; }

		[SkipValidationIfProperty("DeliveryDateSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, Domain.Enums.DeliveryDateSpecificationType.Absolute)]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_AbsoluteDeliveryDate_Date")]
		[DynamicDateRange(1, 1825, ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_AbsoluteDeliveryDate_Range")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_AbsoluteDeliveryDate")]
		public DateTime? AbsoluteDeliveryDate { get; set; }

		[SkipValidationIfProperty("DeliveryDateSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, Domain.Enums.DeliveryDateSpecificationType.DaysFromContract)]
		[Numeric(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_DeliveryDaysAfterContract_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_DeliveryDaysAfterContract_Numeric")]
		[Range(1, 9999, ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_DeliveryDaysAfterContract_Range")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_DeliveryDaysAfterContract")]
		public int? DeliveryDaysAfterContract { get; set; }

		#endregion

		#region User-input: Contact

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_ContactName")]
		[StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string ContactName { get; set; }

		[PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_ContactPhone1_PhoneNumber")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_ContactPhone1")]
		[StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string ContactPhone1 { get; set; }

		[PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_ContactPhone2_PhoneNumber")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_ContactPhone2")]
		[StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string ContactPhone2 { get; set; }

		[Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_ContactEmail_Email")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_ContactEmail")]
		[StringLength(100, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string ContactEmail { get; set; }

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_AgencyName")]
		[StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AgencyName { get; set; }

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_AgencyAddress")]
		[StringLength(200, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AgencyAddress { get; set; }

		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_DirectContactIsPossible")]
		public bool DirectContactIsPossible { get; set; }

		[SkipValidationIfProperty("DirectContactIsPossible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_OwnerName")]
		[StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OwnerName { get; set; }

		[SkipValidationIfProperty("DirectContactIsPossible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_OwnerPhone1_PhoneNumber")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_OwnerPhone1")]
		[StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OwnerPhone1 { get; set; }

		[SkipValidationIfProperty("DirectContactIsPossible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_OwnerPhone2_PhoneNumber")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_OwnerPhone2")]
		[StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OwnerPhone2 { get; set; }

		[SkipValidationIfProperty("DirectContactIsPossible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(PropertyContactResources), ErrorMessageResourceName = "Validation_OwnerEmail_Email")]
		[Display(ResourceType = typeof(PropertyContactResources), Name = "Label_OwnerEmail")]
		[StringLength(100, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OwnerEmail { get; set; }

		#endregion

		public static PropertyContactModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertyContactModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

		    result.IsAgencyListing = domain.IsAgencyListing;

			if (domain.ContactInfo != null)
			{
				result.AgencyName = domain.ContactInfo.AgencyName;
				result.AgencyAddress = domain.ContactInfo.AgencyAddress;
				result.ContactName = domain.ContactInfo.ContactName;
				result.ContactPhone1 = domain.ContactInfo.ContactPhone1;
				result.ContactPhone2 = domain.ContactInfo.ContactPhone2;
				result.ContactEmail = domain.ContactInfo.ContactEmail;

				result.DirectContactIsPossible = domain.ContactInfo.OwnerCanBeContacted;
				result.OwnerName = domain.ContactInfo.OwnerName;
				result.OwnerPhone1 = domain.ContactInfo.OwnerPhone1;
				result.OwnerPhone2 = domain.ContactInfo.OwnerPhone2;
				result.OwnerEmail = domain.ContactInfo.OwnerEmail;
			}

			result.AppropriateVisitTimes = domain.AppropriateVisitTimes;
			result.InappropriateVisitTimes = domain.InappropriateVisitTimes;
			result.ShouldCoordinateBeforeVisit = domain.ShouldCoordinateBeforeVisit;
			result.HowToCoordinateBeforeVisit = domain.HowToCoordinateBeforeVisit;

			result.PropertyStatus = domain.PropertyStatus;
			result.RenterContractEndDate = domain.RenterContractEndDate;
			result.DeliveryDateSpecificationType = domain.DeliveryDateSpecificationType;
			result.AbsoluteDeliveryDate = domain.AbsoluteDeliveryDate;
			result.DeliveryDaysAfterContract = domain.DeliveryDaysAfterContract;

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.ContactInfo == null)
				domain.ContactInfo = new PropertyListingContactInfo();

			domain.ContactInfo.AgencyName = AgencyName;
			domain.ContactInfo.AgencyAddress = AgencyAddress;
			domain.ContactInfo.ContactName = ContactName;
			domain.ContactInfo.ContactPhone1 = ContactPhone1;
			domain.ContactInfo.ContactPhone2 = ContactPhone2;
			domain.ContactInfo.ContactEmail = ContactEmail;

			domain.ContactInfo.OwnerCanBeContacted = DirectContactIsPossible;
			domain.ContactInfo.OwnerName = OwnerName;
			domain.ContactInfo.OwnerPhone1 = OwnerPhone1;
			domain.ContactInfo.OwnerPhone2 = OwnerPhone2;
			domain.ContactInfo.OwnerEmail = OwnerEmail;

			domain.AppropriateVisitTimes = AppropriateVisitTimes;
			domain.InappropriateVisitTimes = InappropriateVisitTimes;
			domain.ShouldCoordinateBeforeVisit = ShouldCoordinateBeforeVisit;
			domain.HowToCoordinateBeforeVisit = HowToCoordinateBeforeVisit;

			domain.PropertyStatus = PropertyStatus;
			domain.RenterContractEndDate = RenterContractEndDate;
			domain.DeliveryDateSpecificationType = DeliveryDateSpecificationType;
			domain.AbsoluteDeliveryDate = AbsoluteDeliveryDate;
			domain.DeliveryDaysAfterContract = DeliveryDaysAfterContract;
		}
	}
}