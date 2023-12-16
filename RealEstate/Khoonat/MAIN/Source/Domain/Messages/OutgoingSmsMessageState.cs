namespace JahanJooy.RealEstate.Domain.Messages
{
    public enum OutgoingSmsMessageState : byte
    {
        InQueue = 1,
        Transmitting = 11,
        AwaitingDelivery = 99,
        ErrorInTransmission = 100,
        NotDelivered = 101,
        DeliveryUnknown = 102,
        Delivered = 200,
    }
}