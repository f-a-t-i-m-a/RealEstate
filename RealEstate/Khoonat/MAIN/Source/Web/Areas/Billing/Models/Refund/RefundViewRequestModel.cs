using System.Web.Mvc;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Refund
{
    [Bind(Exclude = "Requests")]
    public class RefundViewRequestModel
    {
        public PagedList<UserRefundRequest> Requests { get; set; }
        public string Page { get; set; }
    }
}