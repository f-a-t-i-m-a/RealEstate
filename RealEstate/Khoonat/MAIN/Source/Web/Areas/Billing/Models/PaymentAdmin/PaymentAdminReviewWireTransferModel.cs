using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.PaymentAdmin
{
    public class PaymentAdminReviewWireTransferModel
    {
        public UserWireTransferPayment Payment { get; set; }
        public UserBillingApplyResult ApplyResult { get; set; }
    }
}