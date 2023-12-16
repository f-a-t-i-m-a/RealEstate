using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.UserBalanceAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class UserBalanceAdminController : CustomControllerBase
    {
        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBalanceService UserBalanceService { get; set; }

        #endregion

        #region Actions for UserBalanceAdministrativeChange entity

        [HttpGet]
        public ActionResult ListAdministrativeChanges(UserBalanceAdminListAdministrativeChangesModel model)
        {
            if (model == null)
                model = new UserBalanceAdminListAdministrativeChangesModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var changesQuery = DbManager.Db.UserBalanceAdministrativeChanges.Include(c => c.CreatedByUser).Include(c => c.ReviewedByUser).Include(c => c.TargetUser);
            changesQuery = ApplyFilterQuery(model, changesQuery);

			model.Changes = PagedList<UserBalanceAdministrativeChange>.BuildUsingPageNumber(changesQuery.Count(), 20, pageNum);
            model.Changes.FillFrom(changesQuery.OrderByDescending(c => c.CreationTime));

            return View(model);
        }

        [HttpGet]
        public ActionResult NewAdministrativeChange()
        {
            var model = new UserBalanceAdminAdministrativeChangeModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("NewAdministrativeChange")]
        public ActionResult NewAdministrativeChangePostback(UserBalanceAdminAdministrativeChangeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var change = new UserBalanceAdministrativeChange
                         {
                             Description = model.Description,
                             AdministrativeNotes = model.AdministrativeNotes,
                             CashDelta = model.CashDelta.GetValueOrDefault(),
                             BonusDelta = model.BonusDelta.GetValueOrDefault(),
                             TargetUserID = model.TargetUserID
                         };

            UserBalanceService.CreateAdministrativeChange(change);
            return RedirectToAction("ListAdministrativeChanges");
        }

        [HttpPost]
        public ActionResult ViewAdministrativeChangeDetailsPopup(long id)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChanges
                .Include(c => c.CreatedByUser)
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (change == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(change);
        }

        [HttpGet]
        public ActionResult EditAdministrativeChange(long id)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChanges.Include(c => c.TargetUser).SingleOrDefault(c => c.ID == id);

            if (change == null)
                return View("_Errors/EntityNotFound");

            var model = new UserBalanceAdminAdministrativeChangeModel
                        {
                            AdministrativeChange = change,
                            ID = change.ID,
                            Description = change.Description,
                            AdministrativeNotes = change.AdministrativeNotes,
                            CashDelta = change.CashDelta,
                            BonusDelta = change.BonusDelta,
                            TargetUserID = change.TargetUserID
                        };

            return View(model);
        }

        [HttpPost]
        [ActionName("EditAdministrativeChange")]
        public ActionResult EditAdministrativeChangePostback(UserBalanceAdminAdministrativeChangeModel model)
        {
            if (!model.ID.HasValue)
                return View("_Errors/EntityNotFound");

            if (!ModelState.IsValid)
            {
                var change = DbManager.Db.UserBalanceAdministrativeChanges.Include(c => c.TargetUser).SingleOrDefault(c => c.ID == model.ID.Value);
                if (change == null)
                    return View("_Errors/EntityNotFound");

                model.AdministrativeChange = change;
                return View(model);
            }

            var updatedChange = new UserBalanceAdministrativeChange
                                {
                                    ID = model.ID.Value,
                                    Description = model.Description,
                                    AdministrativeNotes = model.AdministrativeNotes,
                                    CashDelta = model.CashDelta.GetValueOrDefault(),
                                    BonusDelta = model.BonusDelta.GetValueOrDefault(),
                                };

            UserBalanceService.UpdateAdministrativeChange(updatedChange);
            return RedirectToAction("ListAdministrativeChanges");
        }

        [HttpPost]
        public ActionResult ReviewAdministrativeChangePopup(long id)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChanges
                .Include(c => c.CreatedByUser)
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (change == null)
                return PartialView("_Errors/EntityNotFound");

            if (change.BillingState != BillingSourceEntityState.Pending)
                return PartialView("_Errors/EntityNotFound");

            var effect = UserBalanceService.CalculateEffectOfAdministrativeChange(id);
            if (effect == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(new UserBalanceAdminReviewAdministrativeChangeModel {Change = change, ApplyResult = effect});
        }

        [HttpPost]
        [ActionName("ReviewAdministrativeChangePostback")]
        [SubmitButton("btnConfirm")]
        public ActionResult ConfirmAdministrativeChange(long id)
        {
            UserBalanceService.ReviewAdministrativeChange(id, true);
            return RedirectToAction("ListAdministrativeChanges");
        }

        [HttpPost]
        [ActionName("ReviewAdministrativeChangePostback")]
        [SubmitButton("btnReject")]
        public ActionResult RejectAdministrativeChange(long id)
        {
            UserBalanceService.ReviewAdministrativeChange(id, false);
            return RedirectToAction("ListAdministrativeChanges");
        }

        [HttpPost]
        public ActionResult ReverseAdministrativeChangePopup(long id)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChanges
                .Include(c => c.CreatedByUser)
                .Include(c => c.ReviewedByUser)
                .Include(c => c.TargetUser)
                .Include(c => c.ForwardTransaction)
                .Include(c => c.ReverseTransaction)
                .SingleOrDefault(c => c.ID == id);

            if (change == null)
                return PartialView("_Errors/EntityNotFound");

            if (change.BillingState != BillingSourceEntityState.Applied)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(change);
        }

        [HttpPost]
        [SubmitButton("btnReverse")]
        public ActionResult ReverseAdministrativeChangeConfirmed(long id)
        {
            UserBalanceService.ReverseAdministrativeChange(id);
            return RedirectToAction("ListAdministrativeChanges");
        }

        #endregion

        #region Actions for users report

        public ActionResult ListUsers(UserBalanceAdminListUsersModel model)
        {
            if (model == null)
                model = new UserBalanceAdminListUsersModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var balancesQuery = DbManager.Db.Users.Select(UserBillingBalance.MapFromUsers());
            balancesQuery = ApplyFilterQuery(model, balancesQuery);

			model.Balances = PagedList<UserBillingBalance>.BuildUsingPageNumber(balancesQuery.Count(), 20, pageNum);

            balancesQuery = ApplySortOrder(model, balancesQuery);
            model.Balances.FillFrom(balancesQuery);

            return View(model);
        }

        #endregion

        #region Private helper methods

        private static IQueryable<UserBalanceAdministrativeChange> ApplyFilterQuery(UserBalanceAdminListAdministrativeChangesModel model, IQueryable<UserBalanceAdministrativeChange> changes)
        {
            if (model.CreatedByUserIDFilter.HasValue)
                changes = changes.Where(c => c.CreatedByUserID == model.CreatedByUserIDFilter.Value);

            if (model.ApplyReviewedByUserIDFilter)
                changes = model.ReviewedByUserIDFilter.HasValue
                                    ? changes.Where(c => c.ReviewedByUserID == model.ReviewedByUserIDFilter.Value)
                                    : changes.Where(c => c.ReviewedByUserID == null);

            if (model.TargetUserIDFilter.HasValue)
                changes = changes.Where(c => c.TargetUserID == model.TargetUserIDFilter.Value);

            if (model.BillingStateFilter.HasValue)
                changes = changes.Where(c => c.BillingState == model.BillingStateFilter.Value);

            if (!string.IsNullOrWhiteSpace(model.TextFilter))
                changes = changes.Where(c => c.Description.Contains(model.TextFilter) || c.AdministrativeNotes.Contains(model.TextFilter));

            return changes;
        }

        private static IQueryable<UserBillingBalance> ApplyFilterQuery(UserBalanceAdminListUsersModel model, IQueryable<UserBillingBalance> balances)
        {
            if (model.UserIDFilter.HasValue)
                balances = balances.Where(b => b.User.ID == model.UserIDFilter.Value);

            if (!string.IsNullOrWhiteSpace(model.UserNameFilter))
                balances = balances.Where(b => b.User.LoginName.Contains(model.UserNameFilter) || b.User.DisplayName.Contains(model.UserNameFilter) || b.User.FullName.Contains(model.UserNameFilter));

            if (!string.IsNullOrWhiteSpace(model.UserContactMethodFilter))
                balances = balances.Where(b => b.User.ContactMethods.Any(cm => cm.ContactMethodText.Contains(model.UserContactMethodFilter)));

            if (model.MinCashBalance.HasValue)
                balances = balances.Where(b => b.LastTransaction.CashBalance >= model.MinCashBalance.Value);

            if (model.MaxCashBalance.HasValue)
                balances = balances.Where(b => b.LastTransaction.CashBalance <= model.MaxCashBalance.Value);

            if (model.MinCashTurnover.HasValue)
                balances = balances.Where(b => b.LastTransaction.CashTurnover >= model.MinCashTurnover.Value);

            if (model.MaxCashTurnover.HasValue)
                balances = balances.Where(b => b.LastTransaction.CashTurnover <= model.MaxCashTurnover.Value);

            // TODO: When ZERO is included in the min/max range, what should we do? Check for null?

            return balances;
        }

        private static IQueryable<UserBillingBalance> ApplySortOrder(UserBalanceAdminListUsersModel model, IQueryable<UserBillingBalance> balances)
        {
            if (!model.SortOrder.HasValue)
                return balances.OrderByDescending(b => b.User.LastLogin);

            switch (model.SortOrder.Value)
            {
                case UserBalanceAdminListUsersOrder.ID:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.User.ID)
                        : balances.OrderBy(b => b.User.ID);

                case UserBalanceAdminListUsersOrder.LoginName:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.User.LoginName)
                        : balances.OrderBy(b => b.User.LoginName);

                case UserBalanceAdminListUsersOrder.CreationDate:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.User.CreationDate)
                        : balances.OrderBy(b => b.User.CreationDate);

                case UserBalanceAdminListUsersOrder.LastLogin:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.User.LastLogin)
                        : balances.OrderBy(b => b.User.LastLogin);

                case UserBalanceAdminListUsersOrder.CashBalance:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.LastTransaction.CashBalance)
                        : balances.OrderBy(b => b.LastTransaction.CashBalance);

                case UserBalanceAdminListUsersOrder.BonusBalance:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.LastTransaction.BonusBalance)
                        : balances.OrderBy(b => b.LastTransaction.BonusBalance);

                case UserBalanceAdminListUsersOrder.CashTurnover:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.LastTransaction.CashTurnover)
                        : balances.OrderBy(b => b.LastTransaction.CashTurnover);

                case UserBalanceAdminListUsersOrder.BonusTurnover:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.LastTransaction.BonusTurnover)
                        : balances.OrderBy(b => b.LastTransaction.BonusTurnover);

                case UserBalanceAdminListUsersOrder.LastTransactionTime:
                    return model.SortDescending
                        ? balances.OrderByDescending(b => b.LastTransaction.TransactionTime)
                        : balances.OrderBy(b => b.LastTransaction.TransactionTime);
            }

            return balances.OrderByDescending(b => b.User.LastLogin);
        }

        #endregion
    }
}