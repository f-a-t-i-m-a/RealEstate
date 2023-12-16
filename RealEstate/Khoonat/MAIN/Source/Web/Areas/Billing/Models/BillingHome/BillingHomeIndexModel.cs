using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.BillingHome
{
    public class BillingHomeIndexModel
    {
        public UserBillingBalance Balance { get; set; }
        public long NumberOfTransactions { get; set; }
        public UserBillingTransaction LastTransaction { get; set; }

        public long NumberOfPendingWireTransferPayments { get; set; }
        public long NumberOfCompletedWireTransferPayments { get; set; }
        public long NumberOfCancelledWireTransferPayments { get; set; }
        public decimal TotalCompletedWireTransferPaymentsCashAmount { get; set; }
        public decimal TotalCompletedWireTransferPaymentsBonusAmount { get; set; }

        public long NumberOfPendingElectronicPayments { get; set; }
        public long NumberOfCompletedElectronicPayments { get; set; }
        public long NumberOfCancelledElectronicPayments { get; set; }
        public decimal TotalCompletedElectronicPaymentsCashAmount { get; set; }
        public decimal TotalCompletedElectronicPaymentsBonusAmount { get; set; }

        public long NumberOfPendingRefundRequests { get; set; }
        public long NumberOfCompletedRefundRequests { get; set; }
        public long NumberOfCancelledRefundRequests { get; set; }
        public decimal TotalNonCancelledRefundRequestsAmount { get; set; }
        public decimal TotalClearedRefundRequestsAmount { get; set; }

        public long NumberOfRewardedPromotionalBonuses { get; set; }
        public long NumberOfClaimedPromotionalCoupons { get; set; }
        public decimal TotalRewardedPromotionalBonusAmount { get; set; }
        public decimal TotalClaimedPromotionalCouponAmount { get; set; }
    }
}