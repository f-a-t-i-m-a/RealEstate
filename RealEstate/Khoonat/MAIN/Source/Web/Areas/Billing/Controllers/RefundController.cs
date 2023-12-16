using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.Refund;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [Authorize]
    public class RefundController : CustomControllerBase
    {
        private const int PageSize = 20;

        #region Injected dependencies

        [ComponentPlug]
        public IUserRefundService UserRefundService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingBalanceCache BalanceCache { get; set; }

        #endregion

        [HttpGet]
        public ActionResult ViewRequests(RefundViewRequestModel model)
        {
            if (model == null)
                model = new RefundViewRequestModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query = DbManager.Db.UserRefundRequests.Where(p => p.TargetUserID == User.CoreIdentity.UserId.Value)
                    .OrderByDescending(p => p.CreationTime);

			model.Requests = PagedList<UserRefundRequest>.BuildUsingPageNumber(query.Count(), PageSize, pageNum);
            model.Requests.FillFrom(query);

            return View(model);
        }

        [HttpGet]
        public ActionResult NewRequest(RefundNewRequestModel model)
        {
            if (model == null)
            {
                model = new RefundNewRequestModel();
            }

            long userId = User.CoreIdentity.UserId.GetValueOrDefault();
            model.Balance = BalanceCache[userId];

            return View(model);
        }

        [HttpPost]
        [ActionName("NewRequest")]
        public ActionResult NewRequestPostback(RefundNewRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return NewRequest(model);
            }

            if (!model.RequestedMaximumAmount && model.Amount == null)
            {
                ModelState.AddModelError("Amount", RefundModelResources.Validation_Amount_Required);
                return NewRequest(model);
            }

            if (model.TargetCardNumber == null && model.TargetShebaNumber == null)
            {
                ModelState.AddModelError("TargetCardNumber", RefundModelResources.Validation_Account_Info);
                return NewRequest(model);
            }

            decimal amount;
            long userId = User.CoreIdentity.UserId.GetValueOrDefault();
            model.Balance = BalanceCache[userId];

            if (model.RequestedMaximumAmount)
            {
                amount = model.Balance.CashBalance;
                if (amount <= 0)
                {
                    ModelState.AddModelError("RequestedMaximumAmount", RefundModelResources.Validation_Amount_Range);
                    return NewRequest(model);
                }
            }
            else
            {
                amount = model.Amount.GetValueOrDefault();
                //check the amount request 
                if (amount > model.Balance.CashBalance || amount <= 0)
                {
                    ModelState.AddModelError("Amount", RefundModelResources.Validation_Amount_Range);
                    return NewRequest(model);
                }
            }

            var userRefundRequest = new UserRefundRequest
            {
                RequestedMaximumAmount = model.RequestedMaximumAmount,
                RequestedAmount = amount,
                TargetCardNumber = model.TargetCardNumber,
                TargetShebaNumber = model.TargetShebaNumber,
                TargetAccountHolderName = model.TargetAccountHolderName,
                TargetBank = model.TargetBank.GetValueOrDefault(),
                UserEnteredReason = model.UserEnteredReason,
                UserEnteredDescription = model.UserEnteredDescription,
                TargetUserID = userId
            };

            UserRefundService.CreateRefundRequest(userRefundRequest);
            return RedirectToAction("ViewRequests");
        }
    }
}