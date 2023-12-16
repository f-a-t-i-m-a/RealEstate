using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models.Properties;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSelectAreaRangeModel
	{
		public string Query { get; set; }

		public PropertyType? PropertyType { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 99999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MinUnitArea")]
		public decimal? MinUnitArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 99999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MaxUnitArea")]
		public decimal? MaxUnitArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 99999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MinEstateArea")]
		public decimal? MinEstateArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 99999, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MaxEstateArea")]
		public decimal? MaxEstateArea { get; set; }
	}
}