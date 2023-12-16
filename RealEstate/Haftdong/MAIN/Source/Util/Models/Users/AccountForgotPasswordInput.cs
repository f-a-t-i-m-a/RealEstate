namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    public class AccountForgotPasswordInput
    {
        public string UserName { get; set; }
        public string VerificationPhoneNumber { get; set; }
        public string VerificationEmailAddress { get; set; }
    }
}