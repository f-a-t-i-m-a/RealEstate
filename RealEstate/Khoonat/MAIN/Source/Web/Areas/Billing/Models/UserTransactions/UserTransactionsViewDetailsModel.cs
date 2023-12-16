using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserTransactions
{
    public class UserTransactionsViewDetailsModel
    {
        public UserBillingTransaction Transaction { get; set; }
        public IBillingSourceEntity Source { get; set; }
    }
}