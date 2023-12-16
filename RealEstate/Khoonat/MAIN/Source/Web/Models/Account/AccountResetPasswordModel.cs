using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Models.Account
{
    public class AccountResetPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_LoginName_Required")]
        [Display(ResourceType = typeof(AccountForgotResources), Name = "Label_LoginName")]
		[StringLength(50, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string LoginName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_PasswordResetToken_Required")]
        [Display(ResourceType = typeof(AccountForgotResources), Name = "Label_PasswordResetToken")]
		[StringLength(50, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string PasswordResetToken { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_NewPassword_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_NewPassword_Length", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AccountForgotResources), Name = "Label_NewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_ConfirmNewPassword_Compare")]
        [Display(ResourceType = typeof(AccountForgotResources), Name = "Label_ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}