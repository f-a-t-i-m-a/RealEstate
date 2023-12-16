using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Web.Resources.Models.Properties;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSelectRoomAndParkingCountModel
	{
		public string Query { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[Range(1, 20, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MinRooms")]
		public int? MinRooms { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[Range(1, 20, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Rooms_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MaxRooms")]
		public int? MaxRooms { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Parkings_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Parkings_Numeric")]
		[Range(1, 20, ErrorMessageResourceType = typeof(PropertiesSelectResources), ErrorMessageResourceName = "Validation_Parkings_Range")]
		[Display(ResourceType = typeof(PropertiesSelectResources), Name = "Label_MinParkings")]
		public int? MinParkings { get; set; }

	}
}