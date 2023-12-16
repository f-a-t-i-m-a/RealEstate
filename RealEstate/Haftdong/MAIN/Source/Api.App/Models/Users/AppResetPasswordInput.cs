namespace JahanJooy.RealEstateAgency.Api.App.Models.Users
{
    public class AppResetPasswordInput
    {
        public string UserName { get; set; }
        public string PasswordResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}
