using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.UserTransactionsAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class UserTransactionsAdminController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public ActionResult ListTransactions(UserTransactionsAdminListTransactionsModel model)
        {
            if (model == null)
                model = new UserTransactionsAdminListTransactionsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var txQuery = DbManager.Db.UserBillingTransactions.Include(t => t.User);

            txQuery = ApplyFilterQuery(model, txQuery);

			model.Transactions = PagedList<UserBillingTransaction>.BuildUsingPageNumber(txQuery.Count(), 20, pageNum);
            model.Transactions.FillFrom(txQuery.OrderByDescending(t => t.TransactionTime));

            return View(model);
        }

        #region Private helper methods

        private static IQueryable<UserBillingTransaction> ApplyFilterQuery(UserTransactionsAdminListTransactionsModel model, IQueryable<UserBillingTransaction> transactions)
        {
            if (model.UserIDFilter.HasValue)
                transactions = transactions.Where(t => t.UserID == model.UserIDFilter.Value);

            if (model.SourceTypeFilter.HasValue)
                transactions = transactions.Where(t => t.SourceType == model.SourceTypeFilter.Value);

            if (model.SourceIDFilter.HasValue)
                transactions = transactions.Where(t => t.SourceID == model.SourceIDFilter.Value);

            if (model.IsReverseFilter.HasValue)
                transactions = transactions.Where(t => t.IsReverse == model.IsReverseFilter.Value);

            if (model.MinCashDeltaFilter.HasValue)
                transactions = transactions.Where(t => t.CashDelta >= model.MinCashDeltaFilter.Value);

            if (model.MaxCashDeltaFilter.HasValue)
                transactions = transactions.Where(t => t.CashDelta <= model.MaxCashDeltaFilter.Value);

            if (model.MinBonusDeltaFilter.HasValue)
                transactions = transactions.Where(t => t.BonusDelta >= model.MinBonusDeltaFilter.Value);

            if (model.MaxBonusDeltaFilter.HasValue)
                transactions = transactions.Where(t => t.BonusDelta <= model.MaxBonusDeltaFilter.Value);

            return transactions;
        }

        #endregion
    }
}