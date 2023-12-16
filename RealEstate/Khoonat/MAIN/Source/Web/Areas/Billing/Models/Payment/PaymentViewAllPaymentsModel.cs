using System.Web.Mvc;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    [Bind(Exclude = "Payments")]
    public class PaymentViewAllPaymentsModel
    {
        public PagedList<PaymentViewAllPaymentsItemModel> Payments { get; set; }
        public string Page { get; set; }
    }
}