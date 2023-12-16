using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.PaymentAdmin
{
    public class PaymentAdminListWireTransfersModel
    {
        public PagedList<UserWireTransferPayment> Payments { get; set; }
        public string Page { get; set; }

        public long? TargetUserIDFilter { get; set; }
        public bool ApplyReviewedByUserIDFilter { get; set; }
        public long? ReviewedByUserIDFilter { get; set; }
        public BillingSourceEntityState? BillingStateFilter { get; set; }
        public string TextFilter { get; set; }
        public IranianBank? SourceBankFilter { get; set; }
    }
}