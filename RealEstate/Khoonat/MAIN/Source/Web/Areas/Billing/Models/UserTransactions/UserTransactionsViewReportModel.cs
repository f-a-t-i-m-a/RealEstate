using System.Web.Mvc;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserTransactions
{
    [Bind(Exclude = "Transactions")]
    public class UserTransactionsViewReportModel
    {
        public PagedList<UserBillingTransaction> Transactions { get; set; }
        public string Page { get; set; }
    }
}