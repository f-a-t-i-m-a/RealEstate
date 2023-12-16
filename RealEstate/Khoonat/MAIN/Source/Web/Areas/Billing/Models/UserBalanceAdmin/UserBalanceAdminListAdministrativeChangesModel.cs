using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserBalanceAdmin
{
    public class UserBalanceAdminListAdministrativeChangesModel
    {
        public PagedList<UserBalanceAdministrativeChange> Changes { get; set; }
        public string Page { get; set; }

        public long? CreatedByUserIDFilter { get; set; }
        public bool ApplyReviewedByUserIDFilter { get; set; }
        public long? ReviewedByUserIDFilter { get; set; }
        public long? TargetUserIDFilter { get; set; }
        public BillingSourceEntityState? BillingStateFilter { get; set; }
        public string TextFilter { get; set; }
    }
}