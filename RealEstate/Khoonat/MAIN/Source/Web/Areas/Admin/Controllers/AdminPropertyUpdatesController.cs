using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminPropertyUpdates;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminPropertyUpdatesController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public ActionResult List(AdminPropertyUpdatesListModel model)
		{
			if (model == null)
				model = new AdminPropertyUpdatesListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;
			var updatesQuery = db.PropertyListingUpdateHistories.Include(h => h.PropertyListing);

			updatesQuery = ApplyFilterQuery(model, updatesQuery);

			model.Updates = PagedList<PropertyListingUpdateHistory>.BuildUsingPageNumber(updatesQuery.Count(), 20, pageNum);
			model.Updates.FillFrom(updatesQuery.OrderByDescending(l => l.UpdateTime));

			return View(model);
		}

		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
				return Error(ErrorResult.EntityNotFound);

			var update = DbManager.Db.PropertyListingUpdateHistories.FirstOrDefault(f => f.ID == id.Value);
			if (update == null)
				return Error(ErrorResult.EntityNotFound);

			update.UpdateDetails = update.UpdateDetails ?? string.Empty;
			if (update.UpdateDetails.StartsWith("<"))
				return Content(update.UpdateDetails, "text/xml");

			return Content(update.UpdateDetails, "text/jsv");
		}

		#region Private helper methods

		private static IQueryable<PropertyListingUpdateHistory> ApplyFilterQuery(AdminPropertyUpdatesListModel model, IQueryable<PropertyListingUpdateHistory> updatesQuery)
		{
			if (model.IdFilter.HasValue)
				updatesQuery = updatesQuery.Where(h => h.ID == model.IdFilter.Value);

			if (model.ApplyUserIdFilter)
				updatesQuery = model.UserIdFilter.HasValue
									? updatesQuery.Where(h => h.UserID == model.UserIdFilter.Value)
									: updatesQuery.Where(h => h.UserID == null);

			if (model.ApplyOwnerUserIdFilter)
				updatesQuery = model.OwnerUserIdFilter.HasValue
									? updatesQuery.Where(h => h.PropertyListing.OwnerUserID == model.OwnerUserIdFilter.Value)
									: updatesQuery.Where(h => h.PropertyListing.OwnerUserID == null);

			if (model.SessionIdFilter.HasValue)
				updatesQuery = updatesQuery.Where(h => h.SessionID == model.SessionIdFilter.Value);

			if (model.ListingIdFilter.HasValue)
				updatesQuery = updatesQuery.Where(h => h.PropertyListingID == model.ListingIdFilter.Value);

			return updatesQuery;
		}

		#endregion
	}
}