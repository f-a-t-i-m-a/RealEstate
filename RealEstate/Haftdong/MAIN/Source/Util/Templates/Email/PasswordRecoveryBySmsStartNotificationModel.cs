﻿using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Users;

namespace JahanJooy.RealEstateAgency.Util.Templates.Email
{
	public class PasswordRecoveryBySmsStartNotificationModel
    {
		public ApplicationUser User { get; set; }
	    public ContactInfo ApplicationUserContactMethod { get; set; }
        public PasswordResetRequest Request { get; set; }
    }
}