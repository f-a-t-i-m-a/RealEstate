using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminAbuseFlags;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminAbuseFlagsController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserFeedbackService UserFeedbackService { get; set; }

        public static string DefaultList(UrlHelper url)
        {
            return url.Action("List", "AdminAbuseFlags");
        }

		public ActionResult List(AdminAbuseFlagsListModel model)
		{
			if (model == null)
				model = new AdminAbuseFlagsListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;
			var flagsQuery = db.AbuseFlags;

			flagsQuery = ApplyFilterQuery(model, flagsQuery);

			model.Flags = PagedList<AbuseFlag>.BuildUsingPageNumber(flagsQuery.Count(), 20, pageNum);
			model.Flags.FillFrom(flagsQuery.OrderByDescending(l => l.ReportDate));

			return View(model);
		}

		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
				return Error(ErrorResult.EntityNotFound);

			var flag = DbManager.Db.AbuseFlags.FirstOrDefault(f => f.ID == id.Value);
			if (flag == null)
				return Error(ErrorResult.EntityNotFound);

			return View(flag);
		}

        [HttpPost]
        [ActionName("ReviewAction")]
        [SubmitButton("btnClearApproval")]
        public ActionResult ClearApproval(long id)
        {
            UserFeedbackService.ReviewAbuse(id, null);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ActionName("ReviewAction")]
        [SubmitButton("btnApprove")]
        public ActionResult Approve(long id)
        {
			UserFeedbackService.ReviewAbuse(id, true);
            return NextForReview(id);
        }

        [HttpPost]
        [ActionName("ReviewAction")]
        [SubmitButton("btnReject")]
        public ActionResult Reject(long id)
        {
			UserFeedbackService.ReviewAbuse(id, false);
            return NextForReview(id);
        }

        [HttpPost]
        [ActionName("ReviewAction")]
        [SubmitButton("btnNext")]
        public ActionResult NextForReview(long id)
        {
            var db = DbManager.Db;
            var next = db.AbuseFlagsDbSet
                .Where(pl => !pl.Approved.HasValue && pl.ID > id)
                .OrderBy(pl => pl.ID)
                .Select(pl => (long?)pl.ID)
                .FirstOrDefault();

            return next.HasValue ? (ActionResult)RedirectToAction("Details", new { id = next.Value }) : Redirect(DefaultList(Url));
        }

        [HttpPost]
        [ActionName("ReviewAction")]
        [SubmitButton("btnPrevious")]
        public ActionResult PreviousForReview(long id)
        {
            var db = DbManager.Db;
            var prev = db.AbuseFlagsDbSet
                .Where(pl => !pl.Approved.HasValue && pl.ID < id)
                .OrderByDescending(pl => pl.ID)
                .Select(pl => (long?)pl.ID)
                .FirstOrDefault();

            return prev.HasValue ? (ActionResult)RedirectToAction("Details", new { id = prev.Value }) : Redirect(DefaultList(Url));
        }


		#region Private helper methods

		private static IQueryable<AbuseFlag> ApplyFilterQuery(AdminAbuseFlagsListModel model, IQueryable<AbuseFlag> flagsQuery)
		{
			if (model.IdFilter.HasValue)
				flagsQuery = flagsQuery.Where(f => f.ID == model.IdFilter.Value);

			if (model.ApplyUserIdFilter)
				flagsQuery = model.UserIdFilter.HasValue
									? flagsQuery.Where(f => f.ReportedByID == model.UserIdFilter.Value)
									: flagsQuery.Where(f => f.ReportedByID == null);

			if (model.SessionIdFilter.HasValue)
				flagsQuery = flagsQuery.Where(f => f.ReportedInSessionID == model.SessionIdFilter.Value);

			if (model.ReasonFilter.HasValue)
				flagsQuery = flagsQuery.Where(f => f.Reason == model.ReasonFilter.Value);

			if (model.EntityTypeFilter.HasValue)
				flagsQuery = flagsQuery.Where(f => f.EntityType == model.EntityTypeFilter.Value);

			if (model.EntityIdFilter.HasValue)
				flagsQuery = flagsQuery.Where(f => f.EntityID == model.EntityIdFilter.Value);

            if (!model.ApprovedFilter)
                flagsQuery = flagsQuery.Where(f => f.Approved == null);

			return flagsQuery;
		}

		#endregion

	}
}