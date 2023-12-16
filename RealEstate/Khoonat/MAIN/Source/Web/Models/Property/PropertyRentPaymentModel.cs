using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyRentPaymentModel : PropertySectionModelBase
	{
		public PropertyRentPaymentModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#region User-input: Price

		[Numeric(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
		[Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Mortgage_Range")]
		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_Mortgage")]
		public decimal? Mortgage { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Rent_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Rent_Numeric")]
		[Range(0, 9999999999, ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_Rent_Range")]
		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_Rent")]
		public decimal? Rent { get; set; }

		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_MortgageAndRentConvertible")]
		public bool MortgageAndRentConvertible { get; set; }

		[SkipValidationIfProperty("MortgageAndRentConvertible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[Numeric(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
		[Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumMortgage_Range")]
		[CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Mortgage", ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumMortgage_CompareToPropertyValue")]
		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_MinimumMortgage")]
		public decimal? MinimumMortgage { get; set; }

		[SkipValidationIfProperty("MortgageAndRentConvertible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
		[Numeric(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
		[Range(0, 9999999999, ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumRent_Range")]
		[CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Rent", ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumRent_CompareToPropertyValue")]
		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_MinimumRent")]
		public decimal? MinimumRent { get; set; }

		#endregion

		#region User-input: Options

		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_IsDischargeGuaranteeChequeRequired")]
		public bool? IsDischargeGuaranteeChequeRequired { get; set; }

		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_DiscountOnBulkPayment")]
		public bool? DiscountOnBulkPayment { get; set; }

		#endregion

		#region User-input: Contract duration

		[CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "MaximumContractDuration", ErrorMessageResourceType = typeof(PropertyRentPaymentResources), ErrorMessageResourceName = "Validation_MinimumContractDuration_CompareToPropertyValue")]
		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_MinimumContractDuration")]
		public ContractDuration? MinimumContractDuration { get; set; }

		[Display(ResourceType = typeof(PropertyRentPaymentResources), Name = "Label_MaximumContractDuration")]
		public ContractDuration? MaximumContractDuration { get; set; }

		#endregion

		public static PropertyRentPaymentModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertyRentPaymentModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (domain.RentConditions != null)
			{
				result.Mortgage = domain.RentConditions.Mortgage;
				result.Rent = domain.RentConditions.Rent;
				result.MortgageAndRentConvertible = domain.RentConditions.MortgageAndRentConvertible;
				result.MinimumMortgage = domain.RentConditions.MinimumMortgage;
				result.MinimumRent = domain.RentConditions.MinimumRent;

				result.IsDischargeGuaranteeChequeRequired = domain.RentConditions.IsDischargeGuaranteeChequeRequired;
				result.DiscountOnBulkPayment = domain.RentConditions.DiscountOnBulkPayment;

				result.MinimumContractDuration = domain.RentConditions.MinimumContractDuration;
				result.MaximumContractDuration = domain.RentConditions.MaximumContractDuration;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.RentConditions == null)
				domain.RentConditions = new RentConditions();

			domain.RentConditions.Mortgage = Mortgage;
			domain.RentConditions.Rent = Rent;
			domain.RentConditions.MortgageAndRentConvertible = MortgageAndRentConvertible;
			domain.RentConditions.MinimumMortgage = MinimumMortgage;
			domain.RentConditions.MinimumRent = MinimumRent;

			domain.RentConditions.IsDischargeGuaranteeChequeRequired = IsDischargeGuaranteeChequeRequired;
			domain.RentConditions.DiscountOnBulkPayment = DiscountOnBulkPayment;

			domain.RentConditions.MinimumContractDuration = MinimumContractDuration;
			domain.RentConditions.MaximumContractDuration = MaximumContractDuration;
		}
	}
}