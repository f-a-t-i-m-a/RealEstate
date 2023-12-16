using System;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    public class PaymentViewAllPaymentsItemModel
    {
        public UserBillingSourceType Type { get; set; }
        public long ID { get; set; }

        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public IranianBank SourceBank { get; set; }

        public BillingSourceEntityState BillingState { get; set; }
        public long? ForwardTransactionID { get; set; }
        public long? ReverseTransactionID { get; set; }
    }
}