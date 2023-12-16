using System.Web.Mvc;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    [Bind(Exclude = "Payments")]
    public class PaymentViewWireTransferPaymentsModel
    {
        public PagedList<UserWireTransferPayment> Payments { get; set; }
        public string Page { get; set; }
    }
}