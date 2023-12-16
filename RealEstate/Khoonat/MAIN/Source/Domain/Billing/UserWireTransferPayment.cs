using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class UserWireTransferPayment : IBillingSourceEntity
    {
        public long ID { get; set; }

        public User ReviewedByUser { get; set; }
        public long? ReviewedByUserID { get; set; }

        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public IranianBank SourceBank { get; set; }
        public string SourceCardNumberLastDigits { get; set; }
        public string SourceAccountHolderName { get; set; }
        // TODO: Should be renamed to UserEnteredDateAndTime or UserEnteredTime
        public DateTime UserEnteredDate { get; set; }
        public string UserEnteredDescription { get; set; }
        public IranianBank TargetBank { get; set; }
        public string FollowUpNumber { get; set; }

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