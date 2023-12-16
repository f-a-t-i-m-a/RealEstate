using System;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminHome;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminHomeController : AdminControllerBase
    {
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public LuceneIndexManager IndexManager { get; set; }

		[HttpGet]
        public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
	    public ActionResult GetStatusSummary()
	    {
			var indexHealth = IndexManager.GetAllHealthStatuses();
			var utcNow = DateTime.UtcNow;

			var model = new AdminHomeIndexModel
			{
				AbuseFlagsQueueSize = DbManager.Db.AbuseFlags.Count(f => !f.Approved.HasValue),
				PropertyListingQueueSize = DbManager.Db.PropertyListings.Count(p => !p.Approved.HasValue),
				PropertyListingPhotosQueueSize = DbManager.Db.PropertyListingPhotos.Count(p => !p.Approved.HasValue),
				IndexesWithErrors = indexHealth.Where(ih => ih.HasErrors).Select(ih => ih.IndexID).ToArray(),
				IndexesNotCommitting = indexHealth.Where(ih => !ih.LastCommitTimeUtc.HasValue || ih.LastCommitTimeUtc.Value + TimeSpan.FromHours(1) < utcNow).Select(ih => ih.IndexID).ToArray()
			};

			return Json(model);
		}
	}
}
