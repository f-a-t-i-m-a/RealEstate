using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.RefundAdmin
{
    public class RefundAdminReviewRequestModel
    {
        public UserRefundRequest Request { get; set; }
        public UserBillingTransaction UserBillingTransaction { get; set; }
    }
}