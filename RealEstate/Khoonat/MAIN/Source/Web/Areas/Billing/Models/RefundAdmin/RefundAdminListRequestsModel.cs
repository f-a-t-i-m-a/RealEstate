using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.RefundAdmin 
{
    public class RefundAdminListRequestsModel
    {
        public PagedList<UserRefundRequest> Requests { get; set; }
        public string Page { get; set; }

        [Display(ResourceType = typeof(RefundAdminModelResources), Name = "Label_TargetUserIDFilter")]
        public long? TargetUserIDFilter { get; set; }
        public BillingSourceEntityState? BillingStateFilter { get; set; }
    }
}