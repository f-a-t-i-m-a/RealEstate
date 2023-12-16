using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class PromotionalBonusCoupon : IBillingSourceEntity
    {
        public long ID { get; set; }

        public User CreatedByUser { get; set; }
        public long CreatedByUserID { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? ClaimTime { get; set; }

        public decimal CouponValue { get; set; }
        public string CouponNumber { get; set; }
        public string CouponPassword { get; set; }

        #region IBillingSourceEntity implementation

        public User TargetUser { get; set; }
        public long? TargetUserID { get; set; }
        public BillingSourceEntityState BillingState { get; set; }
        public UserBillingTransaction ForwardTransaction { get; set; }
        public long? ForwardTransactionID { get; set; }
        public UserBillingTransaction ReverseTransaction { get; set; }
        public long? ReverseTransactionID { get; set; }

        #endregion
    }
}