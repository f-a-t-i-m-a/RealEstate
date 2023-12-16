namespace JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers
{
	public static class ActivityLogDetails
	{
		public static class UserActionDetails
		{
			public const string Register = "Register";
			public const string Login = "Login";
			public const string StartPasswordRecovery = "StartPasswordRecovery";
			public const string CompletePasswordRecovery = "CompletePasswordRecovery";
			public const string AttemptLoginNameRecovery = "AttemptLoginNameRecovery";
			public const string CreateContactMethod = "CreateContactMethod";
			public const string DeleteContactMethod = "DeleteContactMethod";
			public const string StartContactMethodVerification = "StartContactMethodVerification";
			public const string CompleteContactMethodVerification = "CompleteContactMethodVerification";
			public const string CancelContactMethodVerification = "CancelContactMethodVerification";
			public const string CompleteContactMethodVerificationAdministratively = "CompleteContactMethodVerificationAdministratively";
			public const string ChangePassword = "ChangePassword";
			public const string ResetPassword = "ResetPassword";
			public const string EditProfile = "EditProfile";
		}

		public static class PropertyListingActionDetails
		{
			public const string ValidateEditPassword = "ValidateEditPassword";
			public const string AcquireOwnership = "AcquireOwnership";
			public const string ClearApproval = "ClearApproval";
		}

	    public static class BillingActionDetails
	    {
	        public const string Cleared = "Cleared";
	    }
	}
}