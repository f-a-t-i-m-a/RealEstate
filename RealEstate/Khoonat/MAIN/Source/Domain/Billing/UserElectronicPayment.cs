using System;

namespace JahanJooy.RealEstate.Domain.Billing
{
    public class UserElectronicPayment : IBillingSourceEntity
    {
        public long ID { get; set; }

        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public PaymentGatewayProvider PaymentGatewayProvider { get; set; }

        //Bank
       
        public bool? BankPaymentResult { get; set; }
        public decimal? BankAmount { get; set; }
        public string BankInvoiceNumber { get; set; }
        public string BankInvoiceDate { get; set; }
        public long? BankTransactionReferenceID { get; set; }
        public long? BankTraceNumber { get; set; }
        public long? BankReferenceNumber { get; set; }
        public string BankTransactionDate { get; set; }
        public string BankMerchantCode { get; set; }
        public string BankTerminalCode { get; set; }
 



        //VerifyPayment
        public bool? BankVerifyPaymentResult { get; set; }
        public string BankVerifyPaymentResultMessage { get; set; }



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