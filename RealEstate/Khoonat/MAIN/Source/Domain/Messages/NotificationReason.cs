namespace JahanJooy.RealEstate.Domain.Messages
{
    public enum NotificationReason : byte
    {
        General = 0,
        
        UserRegistered = 1,
        ContactMethodVerification = 2,
        PasswordResetRequest = 3,
        LoginNameQuery = 4,
        PasswordRecoveryStarted = 5,
        NonRegisteredEmailUsedForPasswordRecovery = 6,
        NonVerifiedEmailUsedForPasswordRecovery = 7,
        PasswordRecoveryComplete = 8,
        AccountHasBeenLocked = 9,
        AccountHasBeenDisabled = 10,
		AccountPasswordChanged = 11,
		UnusualAccountLogin = 12,
		UnusualAccountActivity = 13,
		SecurityThread = 14,

        ListingRegistered = 21,
        ListingAboutToExpire = 22,

        NewSavedSearchResult = 31,
        
        BalanceRunningLow = 51,
		BalanceDepleted = 52,

		PaymentReceived = 61,
        BonusNotification = 62,

		UnreadChatMessages = 151,

		AdministrativeNotification = 200,
        OperatorRequest = 201,
        Advertisement = 241,
        Unknown = 255
    }
}