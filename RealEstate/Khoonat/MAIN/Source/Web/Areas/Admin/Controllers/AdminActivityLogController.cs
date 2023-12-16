using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Dto.Audit;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminActivityLog;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminActivityLogController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public ActionResult List(AdminActivityLogListModel model)
		{
			if (model == null)
				model = new AdminActivityLogListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;
			var logsQuery = db.ActivityLogs;

			logsQuery = ApplyFilterQuery(model, logsQuery);

			var logsDisplayInfoQuery = ActivityLogDisplayInfo.MakeDetails(logsQuery);
			model.Logs = PagedList<ActivityLogDisplayInfo>.BuildUsingPageNumber(logsQuery.Count(), 20, pageNum);
			model.Logs.FillFrom(logsDisplayInfoQuery.OrderByDescending(l => l.LogDate));

			return View(model);
		}

		#region Private helper methods

		private static IQueryable<ActivityLog> ApplyFilterQuery(AdminActivityLogListModel model, IQueryable<ActivityLog> logsQuery)
		{
			if (model.SessionIdFilter.HasValue)
				logsQuery = logsQuery.Where(l => l.SessionID == model.SessionIdFilter.Value);

			if (model.ActionFilter.HasValue)
				logsQuery = logsQuery.Where(l => l.Action == model.ActionFilter.Value);

			if (model.ApplyAuthenticatedUserIdFilter)
			{
				logsQuery = model.AuthenticatedUserIdFilter.HasValue
					            ? logsQuery.Where(l => l.AuthenticatedUserID == model.AuthenticatedUserIdFilter.Value)
					            : logsQuery.Where(l => l.AuthenticatedUserID == null);
			}

			if (model.TargetEntityFilter.HasValue)
				logsQuery = logsQuery.Where(l => l.TargetEntity == model.TargetEntityFilter.Value);

			if (model.ApplyTargetEntityIdFilter)
			{
				logsQuery = model.TargetEntityIdFilter.HasValue
					            ? logsQuery.Where(l => l.TargetEntityID == model.TargetEntityIdFilter.Value)
					            : logsQuery.Where(l => l.TargetEntityID == null);
			}

			if (model.MinimumReviewWeightFilter.HasValue)
				logsQuery = logsQuery.Where(l => l.ReviewWeight >= model.MinimumReviewWeightFilter.Value);

			return logsQuery;
		}

		#endregion
	}
}