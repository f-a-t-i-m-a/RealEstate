using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.MyProfile;

namespace JahanJooy.RealEstate.Web.Models.MyProfile
{
    [Bind(Exclude = "VerificationInfo")]
    public class MyProfileContactMethodVerificationModel
    {
        public UserContactMethodVerificationInfo VerificationInfo { get; set; }
        public bool VerificationRestarted { get; set; }

        public long ContactMethodID { get; set; }

        [Display(ResourceType = typeof(MyProfileVerifyContactMethodResources), Name = "Label_VerificationSecret")]
        [Required(ErrorMessageResourceType = typeof(MyProfileVerifyContactMethodResources), ErrorMessageResourceName = "Validation_VerificationSecret_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        public string VerificationSecret { get; set; }
    }
}