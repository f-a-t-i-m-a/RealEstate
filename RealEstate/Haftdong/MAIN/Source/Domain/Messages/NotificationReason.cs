namespace JahanJooy.RealEstateAgency.Domain.Messages
{
    public enum NotificationReason:byte
    {
        General = 0,

        UserRegistered = 1,
        ContactMethodVerification = 2,

        Unknown = 255
    }
}