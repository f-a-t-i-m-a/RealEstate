namespace JahanJooy.RealEstate.Domain.Messages
{
    public enum NotificationSourceEntityType : byte
    {
        None = 0,
        User = 1,
        UserContactMethod = 2,
        UserContactMethodVerification = 3,
        PasswordResetRequest = 4,
        PropertyListing = 11,
        PropertyListingPublishHistory = 12,
        SavedPropertySearchSmsNotification = 21,
        PromotionalBonus = 31,
        SponsoredEntity = 41,
        NotificationMessage = 51,
        Userpayment=61
    }
}