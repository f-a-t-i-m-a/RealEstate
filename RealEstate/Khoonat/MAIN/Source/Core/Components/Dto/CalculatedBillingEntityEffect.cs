namespace JahanJooy.RealEstate.Core.Components.Dto
{
    public struct CalculatedBillingEntityEffect
    {
        public static readonly CalculatedBillingEntityEffect Zero = new CalculatedBillingEntityEffect {BonusDelta = 0m, CashDelta = 0m};

        public decimal CashDelta;
        public decimal BonusDelta;
    }
}