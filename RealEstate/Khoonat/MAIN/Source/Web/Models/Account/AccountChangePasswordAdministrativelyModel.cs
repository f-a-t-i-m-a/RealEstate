using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Models.Account
{
    public class AccountChangePasswordAdministrativelyModel
	{
		public long UserId { get; set; }

		[Required(ErrorMessageResourceType = typeof(AccountChangePasswordResources), ErrorMessageResourceName = "Validation_NewPassword_Required")]
		[StringLength(50, ErrorMessageResourceType = typeof(AccountChangePasswordResources), ErrorMessageResourceName = "Validation_NewPassword_Length", MinimumLength = 4)]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(AccountChangePasswordResources), Name = "Label_NewPassword")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessageResourceType = typeof(AccountChangePasswordResources), ErrorMessageResourceName = "Validation_Compare_ConfirmPassword")]
		[Display(ResourceType = typeof(AccountChangePasswordResources), Name = "Label_ConfirmPassword")]
		public string ConfirmPassword { get; set; }
	}
}