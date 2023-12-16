using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.UserTransactions;
using log4net;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [Authorize]
    public class UserTransactionsController : CustomControllerBase
    {
        private const int PageSize = 20;
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserTransactionsController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [HttpGet]
        public ActionResult ViewReport(UserTransactionsViewReportModel model)
        {
            if (model == null)
                model = new UserTransactionsViewReportModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            IQueryable<UserBillingTransaction> txQuery = DbManager.Db.UserBillingTransactions.Where(t => t.UserID == User.CoreIdentity.UserId.Value);

			model.Transactions = PagedList<UserBillingTransaction>.BuildUsingPageNumber(txQuery.Count(), PageSize, pageNum);
            model.Transactions.FillFrom(txQuery.OrderByDescending(t => t.TransactionTime));

            return View(model);
        }

        [HttpPost]
        public ActionResult ViewDetailsPopup(long id)
        {
            var transaction = DbManager.Db.UserBillingTransactions
                 .Include(s => s.User)
                 .SingleOrDefault(t => t.ID == id);

            if (transaction == null || transaction.UserID != User.CoreIdentity.UserId.GetValueOrDefault() && !User.IsOperator)
                return Error(ErrorResult.EntityNotFound);

            var source = LoadBillingSourceEntity(transaction.SourceType, transaction.SourceID);
            return PartialView(new UserTransactionsViewDetailsModel {Transaction = transaction, Source = source});
        }

        private IBillingSourceEntity LoadBillingSourceEntity(UserBillingSourceType sourceType, long sourceID)
        {
            switch (sourceType)
            {
                case UserBillingSourceType.UserWireTransferPayment:
                    return DbManager.Db.UserWireTransferPayments
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.ReviewedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.UserElectronicPayment:
                    return DbManager.Db.UserElectronicPayments
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                       // .Include(s => s.ReviewedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.PromotionalBonus:
                    return DbManager.Db.PromotionalBonuses
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.CreatedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.PromotionalBonusCoupon:
                    return DbManager.Db.PromotionalBonusCoupons
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.CreatedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.SavedSearchSmsNotificationBilling:
                    return DbManager.Db.SavedSearchSmsNotificationBillings
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.SavedPropertySearchPromotionalSms:
                    return DbManager.Db.SavedPropertySearchPromotionalSmses
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.PropertyListing)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.UserRefundRequest:
                    return DbManager.Db.UserRefundRequests
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.ReviewedByUser)
                        .Include(s => s.ClearedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.SavedPropertySearchPromotionalSmsNotDeliveredReturn:
                    return DbManager.Db.SavedPropertySearchPromotionalSmsNotDeliveredReturns
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.PropertyListing)
                        .Include(s => s.PromotionalSms)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.UserBalanceAdministrativeChange:
                    return DbManager.Db.UserBalanceAdministrativeChanges
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.CreatedByUser)
                        .Include(s => s.ReviewedByUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.SponsoredEntityImpressionBilling:
                    return DbManager.Db.SponsoredEntityImpressionBillings
                        .Include(s=>s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s=>s.SponsoredEntity.BilledUser)
                        .SingleOrDefault(s => s.ID == sourceID);

                case UserBillingSourceType.SponsoredEntityClickBilling:
                    return DbManager.Db.SponsoredEntityClickBillings
                        .Include(s => s.ForwardTransaction)
                        .Include(s => s.ReverseTransaction)
                        .Include(s => s.TargetUser)
                        .Include(s => s.SponsoredEntity.BilledUser)
                        .SingleOrDefault(s => s.ID == sourceID);
            }

            return null;
        }
    }
}