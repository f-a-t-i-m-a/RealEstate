namespace JahanJooy.RealEstate.Domain.Billing
{
    public enum UserBillingSourceType : byte
    {
        UserWireTransferPayment = 1,
        UserElectronicPayment = 2,
        PromotionalBonus = 3,
        PromotionalBonusCoupon = 4,
        SavedSearchSmsNotificationBilling = 11,
        SavedPropertySearchPromotionalSms = 12,
        SponsoredEntityImpressionBilling=21,
        SponsoredEntityClickBilling=22,

        UserRefundRequest = 101,
        SavedPropertySearchPromotionalSmsNotDeliveredReturn = 112,

        
        UserBalanceAdministrativeChange = 255
    }
}