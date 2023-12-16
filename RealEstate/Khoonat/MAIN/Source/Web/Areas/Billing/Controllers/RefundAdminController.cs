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
using JahanJooy.RealEstate.Web.Areas.Billing.Models.RefundAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class RefundAdminController : CustomControllerBase
    {
        private const int PageSize = 20;

        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserRefundService UserRefundService { get; set; }

        #endregion

        [HttpGet]
        public ActionResult ListRequests(RefundAdminListRequestsModel model)
        {
            if (model == null)
                model = new RefundAdminListRequestsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query =
                DbManager.Db.UserRefundRequests.Include(c => c.ReviewedByUser)
                    .Include(c => c.TargetUser)
                    .Include(c => c.ClearedByUser);
            query = ApplyFilterQuery(model, query);

			model.Requests = PagedList<UserRefundRequest>.BuildUsingPageNumber(query.Count(), PageSize, pageNum);
            model.Requests.FillFrom(query.OrderByDescending(c => c.CreationTime));

            return View(model);
        }

        [HttpPost]
        public ActionResult ReviewRequestPopup(long id)
        {
            var request = DbManager.Db.UserRefundRequests
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ClearedByUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (request == null)
                return PartialView("_Errors/EntityNotFound");

            if (request.BillingState != BillingSourceEntityState.Applied)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(new RefundAdminReviewRequestModel
            {
                Request = request,
                UserBillingTransaction = request.ForwardTransaction
            });
        }

        [HttpPost]
        public ActionResult PerformPaymentPopUp(long id)
        {
            var request = DbManager.Db.UserRefundRequests
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ClearedByUser)
                .SingleOrDefault(c => c.ID == id);

            if (request == null)
                return PartialView("_Errors/EntityNotFound");

            if (request.BillingState != BillingSourceEntityState.Applied)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(new RefundAdminPerformPaymentModel {Request = request});
        }

        [HttpPost]
        [ActionName("ReviewRefundRequestPostback")]
        [SubmitButton("btnConfirm")]
        public ActionResult ConfirmRefundRequest(long id)
        {
            UserRefundService.ReviewRefundRequest(id, true);
            return RedirectToAction("ListRequests");
        }

        [HttpPost]
        [ActionName("ReviewRefundRequestPostback")]
        [SubmitButton("btnReject")]
        public ActionResult RejectRefundRequest(long id)
        {
            UserRefundService.ReviewRefundRequest(id, false);
            return RedirectToAction("ListRequests");
        }

        [HttpPost]
        [SubmitButton("btnPerformPayment")]
        public ActionResult PerformPayment(long id)
        {
            UserRefundService.PerformPayment(id);
            return RedirectToAction("ListRequests");
        }

        #region Private helper methods

        private static IQueryable<UserRefundRequest> ApplyFilterQuery(RefundAdminListRequestsModel model,
            IQueryable<UserRefundRequest> requests)
        {
            if (model.TargetUserIDFilter.HasValue)
                requests = requests.Where(p => p.TargetUserID == model.TargetUserIDFilter.Value);

            if (model.BillingStateFilter.HasValue)
                requests = requests.Where(p => p.BillingState == model.BillingStateFilter.Value);

            return requests;
        }

        #endregion
    }
}