using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    public class UserApplicationVerifyContactMethodInput
    {
        public string UserID { get; set; }
        public string ContactMethodID { get; set; }
        public string VerificationSecret { get; set; }
    }
}