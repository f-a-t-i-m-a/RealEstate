namespace JahanJooy.RealEstate.Domain.Billing
{
    public enum BillingSourceEntityState : byte
    {
        Pending = 1,
        PartiallyApplied = 30,
        Applied = 50,
        Cancelled = 101,
        Reversed = 102
    }
}