using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminVisitors;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminVisitorsController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public ActionResult List(AdminVisitorsListModel model)
		{
			if (model == null)
				model = new AdminVisitorsListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;
			var visitorsQuery = db.UniqueVisitors;

			visitorsQuery = ApplyFilterQuery(model, visitorsQuery);
			var detailedVisitorsQuery =
				visitorsQuery.Select(v => new AdminVisitorsVisitorModel
					                          {
						                          ID = v.ID,
						                          CreationDate = v.CreationDate,
						                          LastVisitDate = v.LastVisitDate,
						                          UniqueIdentifier = v.UniqueIdentifier,
						                          SessionCount = v.HttpSessions.Count(),
												  UniqueUsers = v.HttpSessions.Select(s => s.UserID).Distinct().Count(),
												  FirstUserAgent = v.HttpSessions.FirstOrDefault().UserAgent
					                          });

			model.Visitors = PagedList<AdminVisitorsVisitorModel>.BuildUsingPageNumber(detailedVisitorsQuery.Count(), 20, pageNum);
			model.Visitors.FillFrom(detailedVisitorsQuery.OrderByDescending(l => l.LastVisitDate));

			return View(model);
		}

		#region Private helper methods

		private static IQueryable<UniqueVisitor> ApplyFilterQuery(AdminVisitorsListModel model, IQueryable<UniqueVisitor> visitorsQuery)
		{
			if (model.IdFilter.HasValue)
				visitorsQuery = visitorsQuery.Where(v => v.ID == model.IdFilter.Value);

			if (model.UserIdFilter.HasValue)
				visitorsQuery = visitorsQuery.Where(v => v.HttpSessions.Any(s => s.UserID == model.UserIdFilter.Value));

			if (!string.IsNullOrWhiteSpace(model.UniqueIdentifierFilter))
				visitorsQuery = visitorsQuery.Where(v => v.UniqueIdentifier.Contains(model.UniqueIdentifierFilter));

			return visitorsQuery;
		}

		#endregion
	}
}