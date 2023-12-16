using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminOperationsController : AdminControllerBase
	{
		[ComponentPlug]
		public IComposer Composer { get; set; }

		[ComponentPlug]
		public IOperationsService OperationsService { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ActionName("RefreshCache")]
		public ActionResult RefreshCachePostback()
		{
			Composer.GetComponent<IVicinityCache>().InvalidateAll();
			return RedirectToAction("Index", "AdminHome");
		}

		[HttpPost]
		public ActionResult RebuildAgencyIndex()
		{
			OperationsService.RebuildAgencyIndex();
			return RedirectToAction("Index", "AdminOperations");
		}

		[HttpPost]
		public ActionResult RebuildPropertyListingIndex()
		{
			OperationsService.RebuildPropertyListingIndex();
			return RedirectToAction("Index", "AdminOperations");
		}

		[HttpPost]
		public ActionResult RebuildPropertyRequestIndex()
		{
			OperationsService.RebuildPropertyRequestIndex();
			return RedirectToAction("Index", "AdminOperations");
		}

        [HttpPost]
        public ActionResult RecalculateUserTransactions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecalculateUserTransactionsPostback(long userId)
        {
	        OperationsService.RecalculateUserTransactions(userId);
            return RedirectToAction("Index", "AdminOperations");
        }

		[HttpPost]
		public ActionResult RebuildPropertyListingPhotos()
		{
			return View();
		}

		[HttpPost]
		public ActionResult RebuildPropertyListingPhotosPostback(long fromId, long toId)
		{
			OperationsService.RebuildPropertyListingPhotos(fromId, toId);
			return RedirectToAction("Index", "AdminOperations");
		}

		[HttpPost]
		public ActionResult RebuildPropertyListingsVicinityHierarchyString()
		{
			OperationsService.RebuildPropertyListingsVicinityHierarchyString();
			return RedirectToAction("Index", "AdminOperations");
		}

		[HttpPost]
		public ActionResult UpdatePropertyListingsGeoLocationFromVicinities()
		{
			OperationsService.UpdatePropertyListingsGeoLocationFromVicinities();
			return RedirectToAction("Index", "AdminOperations");
		}
	}
}