using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Components.Dto;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserBalanceAdmin
{
    public class UserBalanceAdminListUsersModel
    {
        public PagedList<UserBillingBalance> Balances { get; set; }
        public string Page { get; set; }

        public UserBalanceAdminListUsersOrder? SortOrder { get; set; }
        public bool SortDescending { get; set; }
        
        public long? UserIDFilter { get; set; }
        public string UserNameFilter { get; set; }
        public string UserContactMethodFilter { get; set; }

        public decimal? MinCashBalance { get; set; }
        public decimal? MaxCashBalance { get; set; }
        public decimal? MinCashTurnover { get; set; }
        public decimal? MaxCashTurnover { get; set; }
    }

    public enum UserBalanceAdminListUsersOrder
    {
        ID,
        LoginName,
        CreationDate,
        LastLogin,
        CashBalance,
        BonusBalance,
        CashTurnover,
        BonusTurnover,
        LastTransactionTime
    }
}