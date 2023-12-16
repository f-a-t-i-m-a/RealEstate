using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Models.Account
{
	public class AccountLogOnModel
	{
		[Required(ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Required_LoginName")]
		[StringLength(50, ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Length_LoginName", MinimumLength = 4)]
		[Display(ResourceType = typeof(AccountLogOnResources), Name = "Label_LoginName")]
		public string LoginName { get; set; }

		[Required(ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Required_Password")]
		[StringLength(50, ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Length_Password", MinimumLength = 4)]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(AccountLogOnResources), Name = "Label_Password")]
		public string Password { get; set; }

		[Display(ResourceType = typeof(AccountLogOnResources), Name = "Label_RememberMe")]
		public bool RememberMe { get; set; }

		[Display(ResourceType = typeof(AccountLogOnResources), Name = "Label_AcquireOwnedProperties")]
		public bool AcquireOwnedProperties { get; set; } 
	}
}