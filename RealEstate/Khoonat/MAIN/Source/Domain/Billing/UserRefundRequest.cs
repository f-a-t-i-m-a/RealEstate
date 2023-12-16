using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class UserRefundRequest : IBillingSourceEntity
    {
        public long ID { get; set; }

        public User ReviewedByUser { get; set; }
        public long? ReviewedByUserID { get; set; }
        public User ClearedByUser { get; set; }
        public long? ClearedByUserID { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public bool RequestedMaximumAmount { get; set; }
        public decimal RequestedAmount { get; set; }

        // TODO: the following two properties are not yet being used.
        public decimal? DeductibleBankTransactionFee { get; set; }
        public decimal? PayableAmount { get; set; }

        public string TargetCardNumber { get; set; }
        public string TargetShebaNumber { get; set; }
        public IranianBank TargetBank { get; set; }
        public string TargetAccountHolderName { get; set; }
        public string UserEnteredReason { get; set; }
        public string UserEnteredDescription { get; set; }

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