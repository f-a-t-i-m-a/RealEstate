namespace JahanJooy.RealEstateAgency.Domain.Messages
{
    public enum NotificationSeverity : byte
    {
        Trivial = 0,
        Minor = 50,
        Normal = 100,
        Major = 150,
        Critical = 200
    }
}