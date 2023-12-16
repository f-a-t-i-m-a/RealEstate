namespace JahanJooy.RealEstateAgency.Util.Enums
{
    public static class AuthenticationValidationErrors
    {
        public const string UserNameIsAlreadyTaken = "UserNameIsAlreadyTaken";
        public const string UserNotFound = "UserNotFound";
        public const string AccountIsLocked = "AccountIsLocked";
        public const string PasswordDoesNotMatch = "PasswordDoesNotMatch";
        public const string AccountIsDisabled = "AccountIsDisabled";

        // Password validation
        public const string PasswordCantContainUserName = "PasswordCantContainUserName";
        public const string PasswordCantContainDisplayName = "PasswordCantContainDisplayName";
        public const string PasswordCantContainFullName = "PasswordCantContainFullName";

        // Phone number / email validation
        public const string NotAPossiblePhoneNumber = "NotAPossiblePhoneNumber";
        public const string InvalidPhoneNumber = "InvalidPhoneNumber";
        public const string OnlyNationalNumbersAllowed = "OnlyNationalNumbersAllowed";
        public const string OnlyMobileNumbersAllowed = "OnlyMobileNumbersAllowed";
        public const string InvalidEmailAddress = "InvalidEmailAddress";
        public const string DuplicateContactMethodForUser = "DuplicateContactMethodForUser";
        public const string DuplicateContactMethod = "DuplicateContactMethod";
        public const string ContactMethodShouldBeVerifyable = "ContactMethodShouldBeVerifyable";

        // Verification, Password reset, Login name query
        public const string TooFrequentRequests = "TooFrequentRequests";
        public const string TooFrequentContactMethodVerificationStarts = "TooFrequentContactMethodVerificationStarts";
        public const string VerificationNotStarted = "VerificationNotStarted";
        public const string AlreadyVerified = "AlreadyVerified";
        public const string VerificationDeadlineExpired = "VerificationDeadlineExpired";
        public const string UnsupportedContactMethodType = "UnsupportedContactMethodType";
        public const string InvalidSecret = "InvalidSecret";
        public const string UserVerificationMethodNotSupported = "UserVerificationMethodNotSupported";
        public const string PasswordResetTokensDontMatch = "PasswordResetTokensDontMatch";
        public const string RequestHasExpired = "RequestHasExpired";
    }
}