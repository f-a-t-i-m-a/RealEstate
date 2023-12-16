using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.MyProfile;

namespace JahanJooy.RealEstate.Web.Models.MyProfile
{
	public class MyProfileEditModel
	{
		[Required(ErrorMessageResourceType = typeof(MyProfileEditResources), ErrorMessageResourceName = "Validation_Required_FullName")]
		[Display(ResourceType = typeof(MyProfileEditResources), Name = "Label_FullName")]
		[StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string FullName { get; set; }

		[Required(ErrorMessageResourceType = typeof(MyProfileEditResources), ErrorMessageResourceName = "Validation_Required_DisplayName")]
		[Display(ResourceType = typeof(MyProfileEditResources), Name = "Label_DisplayName")]
		[StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string DisplayName { get; set; }

		[Display(ResourceType = typeof(MyProfileEditResources), Name = "Label_DisplayNameSameAsFullName")]
		public bool DisplayNameSameAsFullName { get; set; }
	}
}