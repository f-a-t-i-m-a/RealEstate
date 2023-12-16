using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Users;

namespace JahanJooy.RealEstateAgency.Util.Templates.Email
{
	public class PasswordRecoveryCompletionNotificationModel
    {
		public ApplicationUser User { get; set; }
	    public ContactInfo ApplicationUserContactMethod { get; set; }
        public PasswordResetRequest Request { get; set; }
	    public ContactMethodType ContactMethodType { get; set; }
    }
}