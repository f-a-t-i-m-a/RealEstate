using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.PaymentAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class PaymentAdminController : CustomControllerBase
    {
        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserPaymentService UserPaymentService { get; set; }

        #endregion

        [HttpGet]
        public ActionResult ListWireTransfers(PaymentAdminListWireTransfersModel model)
        {
            if (model == null)
                model = new PaymentAdminListWireTransfersModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var paymentsQuery = DbManager.Db.UserWireTransferPayments.Include(c => c.ReviewedByUser).Include(c => c.TargetUser);
            paymentsQuery = ApplyFilterQuery(model, paymentsQuery);

			model.Payments = PagedList<UserWireTransferPayment>.BuildUsingPageNumber(paymentsQuery.Count(), 20, pageNum);
            model.Payments.FillFrom(paymentsQuery.OrderByDescending(c => c.CreationTime));

            return View(model);
        }

        [HttpPost]
        public ActionResult ViewWireTransferDetailsPopup(long id)
        {
            var payment = DbManager.Db.UserWireTransferPayments
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (payment == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(payment);
        }

        [HttpPost]
        public ActionResult ReviewWireTransferPopup(long id)
        {
            var payment = DbManager.Db.UserWireTransferPayments
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (payment == null)
                return PartialView("_Errors/EntityNotFound");

            if (payment.BillingState != BillingSourceEntityState.Pending)
                return PartialView("_Errors/EntityNotFound");

            var effect = UserPaymentService.CalculateEffectOfWireTransferPayment(id);
            if (effect == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(new PaymentAdminReviewWireTransferModel { Payment = payment, ApplyResult = effect });
        }

        [HttpPost]
        [ActionName("ReviewWireTransferPostback")]
        [SubmitButton("btnConfirm")]
        public ActionResult ConfirmWireTransfer(long id)
        {
            UserPaymentService.ReviewWireTransferPayment(id, true);
            return RedirectToAction("ListWireTransfers");
        }

        [HttpPost]
        [ActionName("ReviewWireTransferPostback")]
        [SubmitButton("btnReject")]
        public ActionResult RejectWireTransfer(long id)
        {
            UserPaymentService.ReviewWireTransferPayment(id, false);
            return RedirectToAction("ListWireTransfers");
        }

        [HttpPost]
        public ActionResult ReverseWireTransferPopup(long id)
        {
            var payment = DbManager.Db.UserWireTransferPayments
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (payment == null)
                return PartialView("_Errors/EntityNotFound");

            if (payment.BillingState != BillingSourceEntityState.Applied)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(payment);
        }

        [HttpPost]
        [SubmitButton("btnReverse")]
        public ActionResult ReverseWireTransferConfirmed(long id)
        {
            UserPaymentService.ReverseWireTransferPayment(id);
            return RedirectToAction("ListWireTransfers");
        }

        #region Private helper methods

        private static IQueryable<UserWireTransferPayment> ApplyFilterQuery(PaymentAdminListWireTransfersModel model, IQueryable<UserWireTransferPayment> payments)
        {
            if (model.TargetUserIDFilter.HasValue)
                payments = payments.Where(p => p.TargetUserID == model.TargetUserIDFilter.Value);

            if (model.ApplyReviewedByUserIDFilter)
                payments = model.ReviewedByUserIDFilter.HasValue
                                    ? payments.Where(p => p.ReviewedByUserID == model.ReviewedByUserIDFilter.Value)
                                    : payments.Where(p => p.ReviewedByUserID == null);

            if (model.BillingStateFilter.HasValue)
                payments = payments.Where(p => p.BillingState == model.BillingStateFilter.Value);

            if (!string.IsNullOrWhiteSpace(model.TextFilter))
                payments = payments.Where(p => p.SourceCardNumberLastDigits.Contains(model.TextFilter) ||
                                               p.SourceAccountHolderName.Contains(model.TextFilter) ||
                                               p.UserEnteredDescription.Contains(model.TextFilter) ||
                                               p.FollowUpNumber.Contains(model.TextFilter));

            if (model.SourceBankFilter.HasValue)
                payments = payments.Where(p => p.SourceBank == model.SourceBankFilter.Value);

            return payments;
        }

        #endregion
    }
}