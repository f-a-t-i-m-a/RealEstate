using JahanJooy.RealEstateAgency.Domain.Enums.User;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    public class NewSecretForUserApplicationInput
    {
        public string UserID { get; set; }
        public string ContactMethodID { get; set; }
        public ContactMethodType ContactMethodType { get; set; }
    }
   
}
