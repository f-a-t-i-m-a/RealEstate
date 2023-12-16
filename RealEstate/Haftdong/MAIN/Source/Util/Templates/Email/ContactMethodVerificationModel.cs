

using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Users;

namespace JahanJooy.RealEstateAgency.Util.Templates.Email
{
	public class ContactMethodVerificationModel
	{
		public ApplicationUser User { get; set; }
	    public ContactMethodType ContactMethodType { get; set; }
        public string ContactMethodText { get; set; }
        public string VerificationSecret { get; set; }
    }
}