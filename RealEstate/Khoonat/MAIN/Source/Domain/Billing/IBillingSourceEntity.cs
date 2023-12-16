namespace JahanJooy.RealEstate.Domain.Billing
{
    public interface IBillingSourceEntity
    {
        long ID { get; }

        User TargetUser { get; }
        long? TargetUserID { get; }

        BillingSourceEntityState BillingState { get; set; }

        UserBillingTransaction ForwardTransaction { get; set; }
        long? ForwardTransactionID { get; set; }

        UserBillingTransaction ReverseTransaction { get; set; }
        long? ReverseTransactionID { get; set; }
    }
}