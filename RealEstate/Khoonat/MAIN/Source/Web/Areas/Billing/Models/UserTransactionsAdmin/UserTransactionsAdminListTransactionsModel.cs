using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserTransactionsAdmin
{
    public class UserTransactionsAdminListTransactionsModel
    {
        public PagedList<UserBillingTransaction> Transactions { get; set; }
        public string Page { get; set; }

        public long? UserIDFilter { get; set; }
        public UserBillingSourceType? SourceTypeFilter { get; set; }
        public long? SourceIDFilter { get; set; }
        public bool? IsReverseFilter { get; set; }

        public decimal? MinCashDeltaFilter { get; set; }
        public decimal? MaxCashDeltaFilter { get; set; }
        public decimal? MinBonusDeltaFilter { get; set; }
        public decimal? MaxBonusDeltaFilter { get; set; }
    }
}