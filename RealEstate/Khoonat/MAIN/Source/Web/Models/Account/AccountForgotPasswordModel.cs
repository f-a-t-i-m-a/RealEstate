using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Models.Account
{
    public class AccountForgotPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(AccountForgotResources), ErrorMessageResourceName = "Validation_LoginName_Required")]
		[StringLength(50, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		[Display(ResourceType = typeof(AccountForgotResources), Name = "Label_LoginName")]
        public string LoginName { get; set; }

		[Display(ResourceType = typeof(AccountForgotResources), Name = "Label_VerificationPhoneNumber")]
		[StringLength(50, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string VerificationPhoneNumber { get; set; }

		[Display(ResourceType = typeof(AccountForgotResources), Name = "Label_VerificationEmailAddress")]
		[StringLength(150, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string VerificationEmailAddress { get; set; }
	}
}