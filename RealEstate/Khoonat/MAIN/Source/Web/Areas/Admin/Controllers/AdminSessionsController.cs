using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSessions;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminSessionsController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public ActionResult List(AdminSessionsListModel model)
		{
			if (model == null)
				model = new AdminSessionsListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;
			var sessionsQuery = db.HttpSessions;

			sessionsQuery = ApplyFilterQuery(model, sessionsQuery);
			var extendedSessionQuery = sessionsQuery.Select(s => new AdminSessionsSessionModel
				                                                     {
					                                                     ID = s.ID,
																		 Type = s.Type,
					                                                     Start = s.Start,
					                                                     End = s.End,
					                                                     EndReason = s.EndReason,
					                                                     UserAgent = s.UserAgent,
					                                                     StartupUri = s.StartupUri,
					                                                     ReferrerUri = s.ReferrerUri,
					                                                     ClientAddress = s.ClientAddress,
					                                                     HttpSessionID = s.HttpSessionID,
					                                                     PrevHttpSessionID = s.PrevHttpSessionID,
					                                                     GotInteractiveAck = s.GotInteractiveAck,
					                                                     UserID = s.UserID,
					                                                     UniqueVisitorID = s.UniqueVisitorID,
																		 ActivityLogCount = s.ActivityLogs.Count(),
																		 UserLoginName = s.User.LoginName
				                                                     });

			model.Sessions = PagedList<AdminSessionsSessionModel>.BuildUsingPageNumber(extendedSessionQuery.Count(), 20, pageNum);
			model.Sessions.FillFrom(extendedSessionQuery.OrderByDescending(l => l.Start));

			return View(model);
		}

		#region Private helper methods

		private static IQueryable<HttpSession> ApplyFilterQuery(AdminSessionsListModel model, IQueryable<HttpSession> sessionsQuery)
		{
			if (model.IdFilter.HasValue)
				sessionsQuery = sessionsQuery.Where(s => s.ID == model.IdFilter.Value);

			if (model.ApplyUserIdFilter)
				sessionsQuery = model.UserIdFilter.HasValue
					                ? sessionsQuery.Where(s => s.UserID == model.UserIdFilter.Value)
					                : sessionsQuery.Where(s => s.UserID == null);

			if (model.VisitorIdFilter.HasValue)
				sessionsQuery = sessionsQuery.Where(s => s.UniqueVisitorID == model.VisitorIdFilter.Value);

			if (model.ApplyEndReasonFilter)
				sessionsQuery = model.EndReasonFilter.HasValue
									? sessionsQuery.Where(s => s.EndReason == model.EndReasonFilter.Value)
									: sessionsQuery.Where(s => s.EndReason == null);

			if (model.InteractiveSessionFilter.HasValue)
				sessionsQuery = sessionsQuery.Where(s => s.GotInteractiveAck == model.InteractiveSessionFilter.Value);

			if (!string.IsNullOrWhiteSpace(model.HttpSessionIdFilter))
				sessionsQuery = sessionsQuery.Where(s => s.HttpSessionID.Contains(model.HttpSessionIdFilter) || s.PrevHttpSessionID.Contains(model.HttpSessionIdFilter));

			if (!string.IsNullOrWhiteSpace(model.UserAgentFilter))
				sessionsQuery = sessionsQuery.Where(s => s.UserAgent.Contains(model.UserAgentFilter));

			if (!string.IsNullOrWhiteSpace(model.StartupUriFilter))
				sessionsQuery = sessionsQuery.Where(s => s.StartupUri.Contains(model.StartupUriFilter));

			if (!string.IsNullOrWhiteSpace(model.ReferrerUriFilter))
				sessionsQuery = sessionsQuery.Where(s => s.ReferrerUri.Contains(model.ReferrerUriFilter));

			if (!string.IsNullOrWhiteSpace(model.ClientAddressFilter))
				sessionsQuery = sessionsQuery.Where(s => s.ClientAddress.Contains(model.ClientAddressFilter));

			if (model.MinimumActivityCount.HasValue)
				sessionsQuery = sessionsQuery.Where(s => s.ActivityLogs.Count() >= model.MinimumActivityCount.Value);

			return sessionsQuery;
		}

		#endregion
	}
}