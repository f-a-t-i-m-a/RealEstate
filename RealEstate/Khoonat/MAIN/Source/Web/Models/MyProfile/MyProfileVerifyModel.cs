using JahanJooy.RealEstate.Core.Services.Dto;

namespace JahanJooy.RealEstate.Web.Models.MyProfile
{
    public class MyProfileVerifyModel
    {
        public UserContactMethodVerificationInfo PhoneVerificationInfo { get; set; }
        public UserContactMethodVerificationInfo EmailVerificationInfo { get; set; }
        public string VerifiedEmail { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}