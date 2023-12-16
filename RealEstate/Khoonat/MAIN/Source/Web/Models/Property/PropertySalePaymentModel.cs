using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertySalePaymentModel : PropertySectionModelBase
	{
		#region Fields

		private decimal _estateArea;
		private decimal _unitArea;

		#endregion

		#region Initialization

		public PropertySalePaymentModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#endregion

		#region Helper properties for calculations

		public decimal EstateArea
		{
			get { return CanSpecifyPerEstateArea ? _estateArea : 0; }
			set { _estateArea = value; }
		}

		public decimal UnitArea
		{
			get { return CanSpecifyPerUnitArea ? _unitArea : 0; }
			set { _unitArea = value; }
		}

		public bool CanSpecifyPerEstateArea
		{
			get { return (PropertyType.HasValue) && (PropertyType.Value.IsEstateSignificant()); }
		}

		public bool CanSpecifyPerUnitArea
		{
			get { return (PropertyType.HasValue) && (PropertyType.Value.HasUnit()); }
		}

		public decimal? CalculatedPaymentForContract
		{
			get { return (PaymentPercentForContract.HasValue && Price.HasValue) ? RoundPrice(PaymentPercentForContract.Value * Price.Value / 100) : null; }
		}

		public decimal? CalculatedPaymentForDelivery
		{
			get { return (PaymentPercentForDelivery.HasValue && Price.HasValue) ? RoundPrice(PaymentPercentForDelivery.Value * Price.Value / 100) : null; }
		}

		public decimal? CalculatedMaximumDebt
		{
			get { return (CanHaveDebt && PaymentPercentForDebt.HasValue && Price.HasValue) ? RoundPrice(PaymentPercentForDebt.Value * Price.Value / 100) : null; }
		}

		#endregion

		#region User-input: Price

		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_PriceSpecificationType")]
		public SalePriceSpecificationType? PriceSpecificationType { get; set; }


		[SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.Total)]
		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 9999999999999, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_Price")]
		public decimal? Price { get; set; }


		[SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.PerEstateArea)]
		[Numeric(ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
		[Range(1, 9999999999, ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerEstateArea_Range")]
		[Display(ResourceType = typeof(PropertySalePaymentResources), Name = "Label_PricePerEstateArea")]
		public decimal? PricePerEstateArea { get; set; }


		[SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.PerUnitArea)]
		[Numeric(ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
		[Range(1, 9999999999, ErrorMessageResourceType = typeof(PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PricePerUnitArea_Range")]
		[Display(ResourceType = typeof(PropertySalePaymentResources), Name = "Label_PricePerUnitArea")]
		public decimal? PricePerUnitArea { get; set; }

		#endregion

		#region User-input: Payment conditions

		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForContract_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForContract_Numeric")]
		[Range(0, 100, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForContract_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_PaymentPercentForContract")]
		public int? PaymentPercentForContract { get; set; }

		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDelivery_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDelivery_Numeric")]
		[Range(0, 100, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDelivery_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_PaymentPercentForDelivery")]
		public int? PaymentPercentForDelivery { get; set; }

		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_CanHaveDebt")]
		public bool CanHaveDebt { get; set; }

		[SkipValidationIfProperty("CanHaveDebt", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDebt_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDebt_Numeric")]
		[Range(0, 100, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_PaymentPercentForDebt_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_PaymentPercentForDebt")]
		public int? PaymentPercentForDebt { get; set; }

		[SkipValidationIfProperty("CanHaveDebt", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_MinimumMonthlyPaymentForDebt_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_MinimumMonthlyPaymentForDebt_Numeric")]
		[Range(0, 9999999999, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_MinimumMonthlyPaymentForDebt_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_MinimumMonthlyPaymentForDebt")]
		public decimal? MinimumMonthlyPaymentForDebt { get; set; }

		[SkipValidationIfProperty("CanHaveDebt", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_DebtGuaranteeType")]
		public DebtGuaranteeType? DebtGuaranteeType { get; set; }

		#endregion

		#region User-input: Loan

		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_HasTransferableLoan")]
		public bool HasTransferableLoan { get; set; }

		[SkipValidationIfProperty("HasTransferableLoan", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
		[Numeric(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
		[Range(0, 9999999999999, ErrorMessageResourceType = typeof (PropertySalePaymentResources), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Range")]
		[Display(ResourceType = typeof (PropertySalePaymentResources), Name = "Label_TransferableLoanAmount")]
		public decimal? TransferableLoanAmount { get; set; }

		#endregion

		#region Private helper methods

		private decimal? RoundPrice(decimal? price)
		{
			if (!price.HasValue)
				return null;

			return decimal.Round(price.Value, 1);
		}

		#endregion

		public static PropertySalePaymentModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertySalePaymentModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (domain.Estate != null)
				if (domain.Estate.Area.HasValue)
					result._estateArea = domain.Estate.Area.Value;

			if (domain.Unit != null)
				if (domain.Unit.Area.HasValue)
					result._unitArea = domain.Unit.Area.Value;

			if (domain.SaleConditions != null)
			{
				result.PriceSpecificationType = domain.SaleConditions.PriceSpecificationType;
				result.Price = domain.SaleConditions.Price;
				result.PricePerEstateArea = domain.SaleConditions.PricePerEstateArea;
				result.PricePerUnitArea = domain.SaleConditions.PricePerUnitArea;

				result.PaymentPercentForContract = domain.SaleConditions.PaymentPercentForContract;
				result.PaymentPercentForDelivery = domain.SaleConditions.PaymentPercentForDelivery;
				result.CanHaveDebt = domain.SaleConditions.CanHaveDebt;
				result.PaymentPercentForDebt = domain.SaleConditions.PaymentPercentForDebt;
				result.MinimumMonthlyPaymentForDebt = domain.SaleConditions.MinimumMonthlyPaymentForDebt;
				result.DebtGuaranteeType = domain.SaleConditions.DebtGuaranteeType;

				result.HasTransferableLoan = domain.SaleConditions.HasTransferableLoan;
				result.TransferableLoanAmount = domain.SaleConditions.TransferableLoanAmount;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.SaleConditions == null)
				domain.SaleConditions = new SaleConditions();

			domain.SaleConditions.PriceSpecificationType = PriceSpecificationType;
			domain.SaleConditions.Price = Price;
			domain.SaleConditions.PricePerEstateArea = PricePerEstateArea;
			domain.SaleConditions.PricePerUnitArea = PricePerUnitArea;

			domain.SaleConditions.PaymentPercentForContract = PaymentPercentForContract;
			domain.SaleConditions.PaymentPercentForDelivery = PaymentPercentForDelivery;
			domain.SaleConditions.CanHaveDebt = CanHaveDebt;
			domain.SaleConditions.PaymentPercentForDebt = PaymentPercentForDebt;
			domain.SaleConditions.MinimumMonthlyPaymentForDebt = MinimumMonthlyPaymentForDebt;
			domain.SaleConditions.DebtGuaranteeType = DebtGuaranteeType;

			domain.SaleConditions.HasTransferableLoan = HasTransferableLoan;
			domain.SaleConditions.TransferableLoanAmount = TransferableLoanAmount;
		}
	}
}