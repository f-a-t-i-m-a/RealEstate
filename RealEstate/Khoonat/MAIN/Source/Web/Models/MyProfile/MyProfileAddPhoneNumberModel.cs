using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.MyProfile;

namespace JahanJooy.RealEstate.Web.Models.MyProfile
{
	public class MyProfileAddPhoneNumberModel
	{
		[Display(ResourceType = typeof(MyProfileAddContactMethodResources), Name = "Label_PhoneNumber")]
		[StrictPhoneNumber(ErrorMessageResourceType = typeof(MyProfileAddContactMethodResources), ErrorMessageResourceName = "Validation_PhoneNumber_PhoneNumber")]
		[Required(ErrorMessageResourceType = typeof(MyProfileAddContactMethodResources), ErrorMessageResourceName = "Validation_PhoneNumber_Required")]
		[StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string PhoneNumber { get; set; }

		[Display(ResourceType = typeof(MyProfileAddContactMethodResources), Name = "Label_Visibility")]
		public VisibilityLevel Visibility { get; set; }
	}
}