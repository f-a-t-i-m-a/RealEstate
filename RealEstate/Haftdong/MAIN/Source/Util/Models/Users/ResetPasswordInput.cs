namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    public class ResetPasswordInput
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordResetToken { get; set; }
        public string NewPassword { get; set; }
      

    }
}