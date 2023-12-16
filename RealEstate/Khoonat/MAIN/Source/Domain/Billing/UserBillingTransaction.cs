using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class UserBillingTransaction
    {
        public long ID { get; set; }

        public long UserID { get; set; }
        public User User { get; set; }

        public DateTime TransactionTime { get; set; }
        
        public decimal CashDelta { get; set; }
        public decimal BonusDelta { get; set; }

        public decimal CashBalance { get; set; }
        public decimal BonusBalance { get; set; }

        public decimal CashTurnover { get; set; }
        public decimal BonusTurnover { get; set; }

        public UserBillingSourceType SourceType { get; set; }
        public long SourceID { get; set; }

        public bool IsReverse { get; set; }
        public bool IsPartial { get; set; }
        public bool HasPartialInHistory { get; set; }
    }
}