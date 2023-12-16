using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthChangePasswordInputModel : ApiInputModelBase
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string NewPassword { get; set; }
    }
}