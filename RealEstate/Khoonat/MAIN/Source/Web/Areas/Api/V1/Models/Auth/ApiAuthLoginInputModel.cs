using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthLoginInputModel : ApiInputModelBase
    {
        [Required(ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Required_LoginName")]
        [StringLength(50, ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Length_LoginName", MinimumLength = 4)]
        public string LoginName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Required_Password")]
        [StringLength(50, ErrorMessageResourceType = typeof(AccountLogOnResources), ErrorMessageResourceName = "Validation_Length_Password", MinimumLength = 4)]
        public string Password { get; set; }

    }
}