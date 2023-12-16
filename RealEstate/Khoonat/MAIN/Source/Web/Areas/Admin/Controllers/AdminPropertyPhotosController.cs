using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminPropertyPhotos;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminPropertyPhotosController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IPropertyPhotoService PropertyPhotoService { get; set; }

		public static string DefaultList(UrlHelper url)
		{
			return url.Action("List", "AdminPropertyPhotos", new AdminPropertyPhotosListModel { ApprovalStatusFilter = PropertyListingPhotoApprovalStatus.NotApproved, DeletedFilter = false });
		}

		[HttpGet]
		public ActionResult List(AdminPropertyPhotosListModel model)
		{
			if (model == null)
				model = new AdminPropertyPhotosListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			IQueryable<PropertyListingPhoto> photosQuery = DbManager.Db.PropertyListingPhotosDbSet;

			photosQuery = ApplyFilterQuery(model, photosQuery);

			model.Photos = PagedList<PropertyListingPhoto>.BuildUsingPageNumber(photosQuery.Count(), 20, pageNum);
			model.Photos.FillFrom(photosQuery.OrderByDescending(p => p.CreationTime).Include(p => p.PropertyListing));

			return View(model);
		}

		[HttpGet]
		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
				return Error(ErrorResult.EntityNotFound);

			// Use DbSet to include deleted items for admin access
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id.Value);
			if (photo == null)
				return Error(ErrorResult.EntityNotFound);

			return View(photo);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnClearApproval")]
		public ActionResult ClearApproval(long id)
		{
			PropertyPhotoService.SetApproved(id, null);
			return RedirectToAction("Details", new {id});
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnApprove")]
		public ActionResult Approve(long id)
		{
			PropertyPhotoService.SetApproved(id, true);
			return NextForReview(id);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnReject")]
		public ActionResult Reject(long id)
		{
			PropertyPhotoService.SetApproved(id, false);
			return NextForReview(id);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnNext")]
		public ActionResult NextForReview(long id)
		{
			var db = DbManager.Db;
			var next = db.PropertyListingPhotosDbSet
				.Where(plp => plp.CreationTime > db.PropertyListingPhotosDbSet.Where(pl2 => pl2.ID == id).Select(pl2 => pl2.CreationTime).FirstOrDefault() && !plp.DeleteTime.HasValue && !plp.Approved.HasValue)
				.OrderBy(plp => plp.CreationTime)
				.Select(plp => (long?) plp.ID)
				.FirstOrDefault();

			return next.HasValue ? (ActionResult)RedirectToAction("Details", new { id = next.Value }) : Redirect(DefaultList(Url));
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnPrevious")]
		public ActionResult PreviousForReview(long id)
		{
			var db = DbManager.Db;
			var prev = db.PropertyListingPhotosDbSet
				.Where(plp => plp.CreationTime < db.PropertyListingPhotosDbSet.Where(pl2 => pl2.ID == id).Select(pl2 => pl2.CreationTime).FirstOrDefault() && !plp.DeleteTime.HasValue && !plp.Approved.HasValue)
				.OrderByDescending(plp => plp.CreationTime)
				.Select(plp => (long?) plp.ID)
				.FirstOrDefault();

			return prev.HasValue ? (ActionResult)RedirectToAction("Details", new { id = prev.Value }) : Redirect(DefaultList(Url));
		}

		private static IQueryable<PropertyListingPhoto> ApplyFilterQuery(AdminPropertyPhotosListModel model, IQueryable<PropertyListingPhoto> photosQuery)
		{
			if (model.IdFilter.HasValue)
				photosQuery = photosQuery.Where(p => p.ID == model.IdFilter.Value);

			if (model.ListingIdFilter.HasValue)
				photosQuery = photosQuery.Where(p => p.PropertyListingID == model.ListingIdFilter.Value);

			if (model.SessionIdFilter.HasValue)
				photosQuery = photosQuery.Where(p => p.CreatorSessionID == model.SessionIdFilter.Value);

			if (model.ApplyUserIdFilter)
			{
				photosQuery = model.UserIdFilter.HasValue
					              ? photosQuery.Where(p => p.CreatorUserID == model.UserIdFilter.Value)
					              : photosQuery.Where(p => p.CreatorUserID == null);
			}

			if (model.ApplyOwnerUserIdFilter)
			{
				photosQuery = model.OwnerUserIdFilter.HasValue
								 ? photosQuery.Where(p => p.PropertyListing.OwnerUserID == model.OwnerUserIdFilter.Value)
								 : photosQuery.Where(p => p.PropertyListing.OwnerUserID == null);
			}

			if (model.ApprovalStatusFilter.HasValue)
			{
				if (model.ApprovalStatusFilter.Value == PropertyListingPhotoApprovalStatus.NotApproved)
					photosQuery = photosQuery.Where(p => p.Approved == null);
				else if (model.ApprovalStatusFilter.Value == PropertyListingPhotoApprovalStatus.Approved)
					photosQuery = photosQuery.Where(p => p.Approved == true);
				else if (model.ApprovalStatusFilter.Value == PropertyListingPhotoApprovalStatus.Rejected)
					photosQuery = photosQuery.Where(p => p.Approved == false);
			}

			if (model.DeletedFilter.HasValue)
			{
				photosQuery = model.DeletedFilter.Value ? photosQuery.Where(p => p.DeleteTime.HasValue) : photosQuery.Where(p => !p.DeleteTime.HasValue);
			}

			return photosQuery;
		}
	}
}