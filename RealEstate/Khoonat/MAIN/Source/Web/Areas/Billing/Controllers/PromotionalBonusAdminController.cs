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
using JahanJooy.RealEstate.Web.Areas.Billing.Models.PromotionalBonusAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class PromotionalBonusAdminController : CustomControllerBase
    {
        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IPromotionalBonusService PromotionalBonusService { get; set; }

        #endregion

        [HttpGet]
        public ActionResult ListBonuses(PromotionalBonusAdminListBonusesModel model)
        {
            if (model == null)
                model = new PromotionalBonusAdminListBonusesModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var bonusesQuery = DbManager.Db.PromotionalBonuses.Include(c => c.CreatedByUser).Include(c => c.TargetUser);
            bonusesQuery = ApplyFilterQuery(model, bonusesQuery);

			model.Bonuses = PagedList<PromotionalBonus>.BuildUsingPageNumber(bonusesQuery.Count(), 20, pageNum);

            bonusesQuery = ApplySortOrder(model, bonusesQuery);
            model.Bonuses.FillFrom(bonusesQuery);

            return View(model);
        }

        [HttpGet]
        public ActionResult NewBonus()
        {
            var model = new PromotionalBonusAdminBonusModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("NewBonus")]
        public ActionResult NewBonusPostback(PromotionalBonusAdminBonusModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.TargetUserIDs == null || model.TargetUserIDs.Length < 1)
                return RedirectToAction("ListBonuses");

            var bonuses = model.TargetUserIDs.Select(uid => new PromotionalBonus
                                                            {
                                                                Description = model.Description,
                                                                BonusAmount = model.BonusAmount.GetValueOrDefault(),
                                                                TargetUserID = uid
                                                            }).ToList();

            PromotionalBonusService.CreateUserRequestedBonuses(bonuses, model.NotifyUser);
            return RedirectToAction("ListBonuses");
        }

        [HttpPost]
        public ActionResult ViewBonusDetailsPopup(long id)
        {
            var bonus = DbManager.Db.PromotionalBonuses
                .Include(b => b.CreatedByUser)
                .Include(b => b.TargetUser)
                .Include(b => b.ForwardTransaction)
                .Include(b => b.ReverseTransaction)
                .SingleOrDefault(b => b.ID == id);

            if (bonus == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(bonus);
        }

        [HttpPost]
        public ActionResult ReverseBonusPopup(long id)
        {
            var bonus = DbManager.Db.PromotionalBonuses
                .Include(b => b.CreatedByUser)
                .Include(b => b.TargetUser)
                .Include(b => b.ForwardTransaction)
                .Include(b => b.ReverseTransaction)
                .SingleOrDefault(b => b.ID == id);

            if (bonus == null)
                return PartialView("_Errors/EntityNotFound");

            if (bonus.BillingState != BillingSourceEntityState.Applied)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(bonus);
        }

        [HttpPost]
        [SubmitButton("btnReverse")]
        public ActionResult ReverseBonusConfirmed(long id)
        {
            PromotionalBonusService.ReverseBonus(id);
            return RedirectToAction("ListBonuses");
        }
     
        #region Private helper methods

        private static IQueryable<PromotionalBonus> ApplyFilterQuery(PromotionalBonusAdminListBonusesModel model, IQueryable<PromotionalBonus> bonuses)
        {
            if (model.ApplyCreatedByUserIDFilter)
                bonuses = model.CreatedByUserIDFilter.HasValue
                    ? bonuses.Where(c => c.CreatedByUserID == model.CreatedByUserIDFilter.Value)
                    : bonuses.Where(c => c.CreatedByUserID == null);

            if (model.TargetUserIDFilter.HasValue)
                bonuses = bonuses.Where(c => c.TargetUserID == model.TargetUserIDFilter.Value);

            if (model.ReasonFilter.HasValue)
                bonuses = bonuses.Where(c => c.Reason == model.ReasonFilter.Value);

            if (!string.IsNullOrWhiteSpace(model.TextFilter))
                bonuses = bonuses.Where(c => c.Description.Contains(model.TextFilter));

            if (model.MinAmountFilter.HasValue)
                bonuses = bonuses.Where(b => b.BonusAmount >= model.MinAmountFilter.Value);

            if (model.MaxAmountFilter.HasValue)
                bonuses = bonuses.Where(b => b.BonusAmount <= model.MaxAmountFilter.Value);

            return bonuses;
        }

        private static IQueryable<PromotionalBonus> ApplySortOrder(PromotionalBonusAdminListBonusesModel model, IQueryable<PromotionalBonus> bonuses)
        {
            if (!model.SortOrder.HasValue)
                return bonuses.OrderByDescending(b => b.ID);

            switch (model.SortOrder.Value)
            {
                case PromotionalBonusAdminListBonusesOrder.ID:
                    return model.SortDescending
                        ? bonuses.OrderByDescending(b => b.ID)
                        : bonuses.OrderBy(b => b.ID);

                case PromotionalBonusAdminListBonusesOrder.CreationTime:
                    return model.SortDescending
                        ? bonuses.OrderByDescending(b => b.CreationTime)
                        : bonuses.OrderBy(b => b.CreationTime);

                case PromotionalBonusAdminListBonusesOrder.BonusAmount:
                    return model.SortDescending
                        ? bonuses.OrderByDescending(b => b.BonusAmount)
                        : bonuses.OrderBy(b => b.BonusAmount);
            }

            return bonuses.OrderByDescending(b => b.ID);
        }

        #endregion
    }
}