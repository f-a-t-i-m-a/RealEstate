using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models.Properties;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSelectPriceRangeModel
	{
		public string Query { get; set; }

		public IntentionOfOwner? IntentionOfOwner { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePriceMinimum")]
		public decimal? SalePriceMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePriceMaximum")]
		public decimal? SalePriceMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePricePerEstateAreaMinimum")]
		public decimal? SalePricePerEstateAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePricePerEstateAreaMaximum")]
		public decimal? SalePricePerEstateAreaMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePricePerUnitAreaMinimum")]
		public decimal? SalePricePerUnitAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_SalePricePerUnitAreaMaximum")]
		public decimal? SalePricePerUnitAreaMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_RentMortgageMinimum")]
		public decimal? RentMortgageMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_RentMortgageMaximum")]
		public decimal? RentMortgageMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(0, 999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_RentMinimum")]
		public decimal? RentMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(0, 999999999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_RentMaximum")]
		public decimal? RentMaximum { get; set; }
	}
}