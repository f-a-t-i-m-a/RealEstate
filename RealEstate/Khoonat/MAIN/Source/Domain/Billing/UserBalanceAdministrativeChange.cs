using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class UserBalanceAdministrativeChange : IBillingSourceEntity
    {
        public long ID { get; set; }

        public User CreatedByUser { get; set; }
        public long CreatedByUserID { get; set; }
        public User ReviewedByUser { get; set; }
        public long? ReviewedByUserID { get; set; }
        public string Description { get; set; }
        public string AdministrativeNotes { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public decimal CashDelta { get; set; }
        public decimal BonusDelta { get; set; }

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