using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.MyProfile;

namespace JahanJooy.RealEstate.Web.Models.MyProfile
{
	public class MyProfileAddEmailModel
	{
		[DataType(DataType.EmailAddress)]
		[Display(ResourceType = typeof(MyProfileAddContactMethodResources), Name = "Label_Email")]
		[Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(MyProfileAddContactMethodResources), ErrorMessageResourceName = "Validation_Email_Email")]
		[StringLength(150, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		[Required(ErrorMessageResourceType = typeof(MyProfileAddContactMethodResources), ErrorMessageResourceName = "Validation_Email_Required")]
		public string Email { get; set; }

		[Display(ResourceType = typeof(MyProfileAddContactMethodResources), Name = "Label_Visibility")]
		public VisibilityLevel Visibility { get; set; }
	}
}