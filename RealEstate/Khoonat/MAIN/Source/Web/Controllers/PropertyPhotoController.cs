using System.Data.Entity;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Upload;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using System.Linq;
using JahanJooy.RealEstate.Web.Helpers.Property;
using JahanJooy.RealEstate.Web.Models.PropertyPhoto;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class PropertyPhotoController : CustomControllerBase
	{
		[ComponentPlug]
		public IPropertyPhotoService PropertyPhotoService { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[HttpPost]
		public ActionResult Tab(long? id)
		{
			if (!id.HasValue)
				return Error(ErrorResult.EntityNotFound);

			// Use DbSet to be able to load deleted listings as well, in case it is accessed by Admin
			PropertyListing listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(pl => pl.ID == id.Value);
			if (listing == null || !PropertyControllerHelper.CanViewListing(listing))
				return Error(ErrorResult.AccessDenied);

			var photos = BuildPropertyListingPhotosQuery(listing).ToList();
			var model = new PropertyPhotoTabModel
				            {
								Listing = listing,
					            Photos = photos,
								SelectedPhotoID = photos.Select(p => (long?)p.ID).LastOrDefault(),
								IsOwner = PropertyControllerHelper.DetermineIfUserIsOwner(listing)
				            };

			return PartialView("_Property/Tabs/Photos", model);
		}

		[HttpGet]
		public ActionResult GetThumbnailImage(long id)
		{
			// Use DbSet to allow admin to see deleted images
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanViewListingPhoto(photo))
			{
				// Non-owner should not access the details of an unpublished or unapproved property.
				return Error(ErrorResult.AccessDenied);
			}

			return File(PropertyPhotoService.GetThumbnailBytes(photo.StoreItemID), "image/jpeg");
		}

		[HttpGet]
		public ActionResult GetMediumSizeImage(long id)
		{
			// Use DbSet to allow admin to see deleted images
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanViewListingPhoto(photo))
			{
				// Non-owner should not access the details of an unpublished or unapproved property.
				return Error(ErrorResult.AccessDenied);
			}

			return File(PropertyPhotoService.GetMediumSizeBytes(photo.StoreItemID), "image/jpeg");
		}

		[HttpGet]
		public ActionResult GetFullSizeImage(long id)
		{
			// Use DbSet to allow admin to see deleted images
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanViewListingPhoto(photo))
			{
				// Non-owner should not access the details of an unpublished or unapproved property.
				return Error(ErrorResult.AccessDenied);
			}

			return File(PropertyPhotoService.GetFullSizeBytes(photo.StoreItemID), "image/jpeg");
		}

		[HttpPost]
		public ActionResult Upload(long id, FineUpload file)
		{
			var listing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == id);
			if (listing == null)
				return Json(new {success = false, error = "No such property listing"});

			if (!PropertyControllerHelper.CanEditListing(listing))
				return Json(new {success = false, error = "Access denied"});

			var photo = PropertyPhotoService.AddPhoto(id, file.InputStream);
			if (photo == null)
				return Json(new { success = false, error = "Invalid file" });

			return Json(new {success = true, PropertyListingPhotoID = photo.ID});
		}

		[HttpPost]
		public ActionResult EditProperties(long id)
		{
			var photo = DbManager.Db.PropertyListingPhotos.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanEditListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			var model = new PropertyPhotoEditPropertiesModel
				            {
					            Photo = photo,
					            Subject = photo.Subject,
					            Title = photo.Title,
					            Description = photo.Description
				            };

			return PartialView("EditProperties", model);
		}

		[HttpPost]
		public ActionResult EditPropertiesPostback(long id, PropertyPhotoEditPropertiesModel model)
		{
			if (!ModelState.IsValid)
				return EditProperties(id);

			var photo = DbManager.Db.PropertyListingPhotos.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanEditListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			photo.Subject = model.Subject;
			photo.Title = model.Title;
			photo.Description = model.Description;

			PropertyPhotoService.UpdatePhotoProperties(photo);
			return ViewMediumSize(id);
		}

		[HttpPost]
		public ActionResult Delete(long id)
		{
			var photo = DbManager.Db.PropertyListingPhotos.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanEditListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			return PartialView(id);
		}

		[HttpPost]
		public ActionResult DeletePostback(long id)
		{
			var photo = DbManager.Db.PropertyListingPhotos.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);

			if (photo == null || !PropertyControllerHelper.CanEditListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			PropertyPhotoService.DeletePhoto(id);
			return RedirectToAction("ViewDetails", "Property", new {id = photo.PropertyListingID});
		}

		[HttpPost]
		public ActionResult ViewMediumSize(long id)
		{
			var photo = DbManager.Db.PropertyListingPhotos.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == id);
			if (photo == null || !PropertyControllerHelper.CanViewListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			var model = new PropertyPhotoViewMediumSizeModel
				            {
					            Photo = photo,
					            Listing = photo.PropertyListing,
					            IsOwner = PropertyControllerHelper.DetermineIfUserIsOwner(photo.PropertyListing)
				            };

			return PartialView("ViewMediumSize", model);
		}

		[HttpGet]
		public ActionResult ViewFullSize(long photoId)
		{
			// Use DbSet to allow operator to view full size photos
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(plp => plp.PropertyListing).SingleOrDefault(plp => plp.ID == photoId);
			if (photo == null || !PropertyControllerHelper.CanViewListingPhoto(photo))
				return Error(ErrorResult.AccessDenied);

			var query = BuildPropertyListingPhotosQuery(photo.PropertyListing);
			var photoIds = query.Select(plp => plp.ID).ToList();

			int photoIndex = photoIds.IndexOf(photoId);
			var model = new PropertyPhotoViewFullSizeModel
				            {
					            Photo = photo,
					            Listing = photo.PropertyListing,
								NextID = (photoIndex + 1) < photoIds.Count && (photoIndex + 1) >= 0 ? (long?)photoIds[photoIndex + 1] : null,
								PrevID = photoIndex > 0 ? (long?)photoIds[photoIndex - 1] : null,
								Index = photoIndex,
								Count = photoIds.Count
				            };

			return View(model);
		}

		#region Private helper methods

		private IQueryable<PropertyListingPhoto> BuildPropertyListingPhotosQuery(PropertyListing listing)
		{
			var query = DbManager.Db.PropertyListingPhotos.Where(plp => plp.PropertyListingID == listing.ID);
			if (!PropertyControllerHelper.DetermineIfUserIsOwner(listing))
				query = query.Where(plp => plp.Approved.HasValue && plp.Approved.Value);

			query = query.OrderBy(plp => plp.Order);
			return query;
		}

		#endregion
	}
}
