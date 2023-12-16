using System;
using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    public class RedirectedFromGatewayProviderBankModel
    {
//        [Range(0, 100000000, ErrorMessage = "پوزش مارو بپذیرید. درگاه پرداخت به زودی راه اندازی خواهد شد.")]
        public decimal Amount { get; set; }

        public PaymentGatewayProvider PaymentGatewayProvider { get; set; }
//        public int PaymentGatewayProviderID { get; set; }

        public long? TransactionReferenceID { get; set; }

//        public bool? VerifiedByBank { get; set; }
//        public string BankVerificationResponse { get; set; }

//        public string MerchantCode { get; set; }
//        public string TerminalCode { get; set; }

//        public String RedirectAddress { get; set; }
//        public String TimeStamp { get; set; }
        public long InvoiceNumber { get; set; }
        public String InvoiceDate { get; set; }
//        public String Action { get; set; }
//        public string Sign { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public bool InvoiceNumberConflict { get; set; }
        public bool AmountConflict { get; set; }
        public bool MerchantCodeConflict { get; set; }
        public bool TerminalCodeConflict { get; set; }
        public bool CheckTransactionResultResposeError { get; set; }
        public bool VerifyPaymentResponseError { get; set; }
        public bool InvoiceNumberNotFound { get; set; }
        public bool InvoiceDateError { get; set; }
        public bool BillingStateError { get; set; }
        public bool PaymentRequestTooOld { get; set; }
        public bool BillingStateIsAppliedError { get; set; }
    }
}