using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Helpers.Property;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class PropertyPhotoByIndexController : CustomControllerBase
    {
        [ComponentPlug]
        public IPropertyPhotoService PropertyPhotoService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [HttpGet]
        public ActionResult GetThumbnailImage(long propertyListingId, int index)
        {
            var photo = FindPhotoObject(propertyListingId, index);

            if (photo == null)
                return HttpNotFound();

            if (!PropertyControllerHelper.CanViewListingPhoto(photo))
                return Error(ErrorResult.AccessDenied);

            return File(PropertyPhotoService.GetThumbnailBytes(photo.StoreItemID), "image/jpeg");
        }

        [HttpGet]
        public ActionResult GetMediumSizeImage(long propertyListingId, int index)
        {
            var photo = FindPhotoObject(propertyListingId, index);

            if (photo == null)
                return HttpNotFound();

            if (!PropertyControllerHelper.CanViewListingPhoto(photo))
                return Error(ErrorResult.AccessDenied);

            return File(PropertyPhotoService.GetMediumSizeBytes(photo.StoreItemID), "image/jpeg");
        }

        [HttpGet]
        public ActionResult GetFullSizeImage(long propertyListingId, int index)
        {
            var photo = FindPhotoObject(propertyListingId, index);

            if (photo == null)
                return HttpNotFound();

            if (!PropertyControllerHelper.CanViewListingPhoto(photo))
                return Error(ErrorResult.AccessDenied);

            return File(PropertyPhotoService.GetFullSizeBytes(photo.StoreItemID), "image/jpeg");
        }


        private PropertyListingPhoto FindPhotoObject(long propertyListingId, int index)
        {
            var photoQuery = DbManager.Db.PropertyListingPhotosDbSet
                .Include(plp => plp.PropertyListing)
                .Where(plp => plp.PropertyListingID == propertyListingId)
                .Where(plp => plp.Approved.Value && plp.Approved.HasValue && !plp.DeleteTime.HasValue)
                .OrderBy(plp => plp.Order);

            var photo = index <= 1
                ? photoQuery.FirstOrDefault()
                : photoQuery.Skip(index - 1).FirstOrDefault();

            return photo;
        }
    }
}