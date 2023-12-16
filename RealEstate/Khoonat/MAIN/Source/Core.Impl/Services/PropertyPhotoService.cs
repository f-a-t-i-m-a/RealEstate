using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using ImageResizer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Streams;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Property;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
    public class PropertyPhotoService : IPropertyPhotoService, IEagerComponent
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyPhotoService));

		#region Component plugs

		[ComponentPlug]
		public BinaryStorageComponent BinaryStorage { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IActivityLogService ActivityLogService { get; set; }

		#endregion

		#region StoreIdConstants

		private const string UntouchedStoreId = "listingPhoto-Untouched";
		private const string ThumbnailStoreId = "listingPhoto-Thumbnail";
		private const string MediumSizeStoreId = "listingPhoto-MediumSize";
		private const string FullSizeStoreId = "listingPhoto-FullSize";

		#endregion

		#region Image resource constants

		private const string ResourceKeyImageCouldNotBeRetrieved = "JahanJooy.RealEstate.Core.Impl.Resources.ErrorImages.ImageCouldNotBeRetrieved.png";
		private const string ResourceKeyThumbnailCouldNotBeRetrieved = "JahanJooy.RealEstate.Core.Impl.Resources.ErrorImages.ThumbnailCouldNotBeRetrieved.png";

		#endregion

        #region Initialization

	    static PropertyPhotoService()
	    {
	        BinaryStorageComponent.RegisterStoreConfigurationKeys(UntouchedStoreId);
	        BinaryStorageComponent.RegisterStoreConfigurationKeys(ThumbnailStoreId);
	        BinaryStorageComponent.RegisterStoreConfigurationKeys(MediumSizeStoreId);
	        BinaryStorageComponent.RegisterStoreConfigurationKeys(FullSizeStoreId);
	    }

	    // TODO: Replace with OnCompositionComplete
        // All of the code in this section is because of a bug in Composer which prevents
        // having [OnCompositionComplete] on methods of types that get proxied / intercepted.
        // Details in RE-318
        // This code should be fixed/removed after Composer is fixed.

        private bool _initialized = false;

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            BinaryStorage.RegisterStoreId(UntouchedStoreId);
            BinaryStorage.RegisterStoreId(ThumbnailStoreId);
            BinaryStorage.RegisterStoreId(MediumSizeStoreId);
            BinaryStorage.RegisterStoreId(FullSizeStoreId);

            _initialized = true;
        }

        #endregion

        #region IPropertyPhotoService implementation

        public PropertyListingPhoto AddPhoto(long listingId, Stream contents)
		{
            EnsureInitialized();

			var listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(l => l.ID == listingId);
			if (listing == null)
				throw new ArgumentException("No such property listing exists.");
            listing.ModificationDate=DateTime.Now;

			var listingPhoto = new PropertyListingPhoto();

			listingPhoto.PropertyListingID = listingId;
			listingPhoto.StoreItemID = Guid.NewGuid();
			listingPhoto.Approved = ServiceContext.Principal.IsOperator ? (bool?)true : null;
			listingPhoto.CreatorSessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID));
			listingPhoto.CreatorUserID = ServiceContext.Principal.CoreIdentity.IsAuthenticated ? ServiceContext.Principal.CoreIdentity.UserId : null;
			listingPhoto.DeleteTime = null;

			listingPhoto.Subject = null;
			listingPhoto.Title = null;
			listingPhoto.Description = null;
			listingPhoto.Order = DateTime.Now.Ticks;

			byte[] untouchedBytes;
			using (var ms = new MemoryStream())
			{
				try
				{
					ImageBuilder.Current.Build(contents, ms, new ResizeSettings("maxwidth=1600&maxheight=1600&format=jpg&quality=90&autorotate=true"));
				}
				catch (Exception e)
				{
					Log.Error("Could not process uploaded image", e);
					return null;
				}

				listingPhoto.UntouchedLength = ms.Length;
				untouchedBytes = ms.ToArray();
			}

            BinaryStorage.StoreBytes(UntouchedStoreId, listingPhoto.StoreItemID, untouchedBytes);
			RebuildListingPhotoSizes(listingPhoto, listing, untouchedBytes);

			DbManager.Db.PropertyListingPhotosDbSet.Add(listingPhoto);
			DbManager.SaveDefaultDbChanges();

			ActivityLogService.ReportActivity(TargetEntityType.PropertyListingPhoto, listingPhoto.ID, ActivityAction.Create, parentEntity: TargetEntityType.PropertyListing, parentEntityID: listing.ID);
			return listingPhoto;
		}

		public ValidationResult UpdatePhotoProperties(PropertyListingPhoto photo)
		{
			if (ServiceContext.CurrentSession.IsCrawler)
				throw new InvalidOperationException("Not a user-interactive session.");

			var dbPhoto = DbManager.Db.PropertyListingPhotosDbSet
                .Include(l=>l.PropertyListing)
                .SingleOrDefault(l => l.ID == photo.ID);
			if (dbPhoto == null)
				throw new ArgumentException("Photo does not exist");

			if (dbPhoto.DeleteTime.HasValue)
				throw new ArgumentException("Photo is already deleted.");

			dbPhoto.Subject = photo.Subject;
			dbPhoto.Title = photo.Title;
			dbPhoto.Description = photo.Description;
			dbPhoto.Approved = ServiceContext.Principal.IsOperator ? (bool?)true : null;

            dbPhoto.PropertyListing.ModificationDate= DateTime.Now;
			ActivityLogService.ReportActivity(TargetEntityType.PropertyListingPhoto, dbPhoto.ID, ActivityAction.Edit, parentEntity: TargetEntityType.PropertyListing, parentEntityID: dbPhoto.PropertyListingID);

			return ValidationResult.Success;
		}

		public void DeletePhoto(long photoId)
		{
			var dbPhoto = DbManager.Db.PropertyListingPhotosDbSet
                .Include(l=>l.PropertyListing)
                .SingleOrDefault(l => l.ID == photoId);
			if (dbPhoto == null)
				throw new ArgumentException("Photo does not exist");

			if (dbPhoto.DeleteTime.HasValue)
				throw new ArgumentException("Photo is already deleted.");

			dbPhoto.DeleteTime = DateTime.Now;
            dbPhoto.PropertyListing.ModificationDate = DateTime.Now;

            ActivityLogService.ReportActivity(TargetEntityType.PropertyListingPhoto, dbPhoto.ID, ActivityAction.Delete, parentEntity: TargetEntityType.PropertyListing, parentEntityID: dbPhoto.PropertyListingID);
		}

		public void SetApproved(long photoId, bool? approved)
		{
			var listing = DbManager.Db.PropertyListingPhotosDbSet.SingleOrDefault(l => l.ID == photoId);
			if (listing == null)
				throw new ArgumentException("Photo not found.");

			if (listing.DeleteTime.HasValue)
				return;

			if (listing.Approved == approved)
				return;

			listing.Approved = approved;

			if (!approved.HasValue)
			{
				ActivityLogService.ReportActivity(TargetEntityType.PropertyListingPhoto, photoId, ActivityAction.Edit, ActivityLogDetails.PropertyListingActionDetails.ClearApproval);
			}
			else
			{
				ActivityLogService.ReportActivity(TargetEntityType.PropertyListingPhoto, photoId, approved.Value ? ActivityAction.Approve : ActivityAction.Reject);
				ActivityLogService.MarkTaskComplete(TargetEntityType.PropertyListingPhoto, photoId, ActivityAction.Create);
				ActivityLogService.MarkTaskComplete(TargetEntityType.PropertyListingPhoto, photoId, ActivityAction.Edit);
			}
		}

		public void RebuildPhoto(long photoId)
		{
			var photo = DbManager.Db.PropertyListingPhotosDbSet.Include(p => p.PropertyListing).SingleOrDefault(p => p.ID == photoId);
			if (photo == null)
				return;

			var untouchedBytes = GetUntouchedBytes(photo.StoreItemID);
			if (untouchedBytes == null)
				return;

			RebuildListingPhotoSizes(photo, photo.PropertyListing, untouchedBytes);
		}

		public byte[] GetThumbnailBytes(Guid storeItemId)
		{
            EnsureInitialized();

			try
			{
                return BinaryStorage.RetrieveBytes(ThumbnailStoreId, storeItemId);
			}
			catch (Exception e)
			{
				Log.Error("Could not load image ID " + storeItemId + " from store " + ThumbnailStoreId, e);
				return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(), ResourceKeyThumbnailCouldNotBeRetrieved);
			}
		}

		public byte[] GetMediumSizeBytes(Guid storeItemId)
		{
            EnsureInitialized();

			try
			{
                return BinaryStorage.RetrieveBytes(MediumSizeStoreId, storeItemId);
			}
			catch (Exception e)
			{
				Log.Error("Could not load image ID " + storeItemId + " from store " + MediumSizeStoreId, e);
				return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(), ResourceKeyImageCouldNotBeRetrieved);
			}
		}

		public byte[] GetFullSizeBytes(Guid storeItemId)
		{
            EnsureInitialized();

			try
			{
                return BinaryStorage.RetrieveBytes(FullSizeStoreId, storeItemId);
			}
			catch (Exception e)
			{
				Log.Error("Could not load image ID " + storeItemId + " from store " + FullSizeStoreId, e);
				return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(), ResourceKeyImageCouldNotBeRetrieved);
			}
		}

		public byte[] GetUntouchedBytes(Guid storeItemId)
		{
            EnsureInitialized();

			try
			{
                return BinaryStorage.RetrieveBytes(UntouchedStoreId, storeItemId);
			}
			catch (Exception e)
			{
				Log.Error("Could not load image ID " + storeItemId + " from store " + UntouchedStoreId, e);
				return null;
			}
		}

		#endregion

		#region Private helper methods

		private void RebuildListingPhotoSizes(PropertyListingPhoto listingPhoto, PropertyListing listing, byte[] sourceImageBytes)
		{
            EnsureInitialized();

			using (var ms = new MemoryStream())
			{
				ImageBuilder.Current.Build(sourceImageBytes, ms, new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));
				listingPhoto.ThumbnailLength = ms.Length;
                BinaryStorage.StoreBytes(ThumbnailStoreId, listingPhoto.StoreItemID, ms.ToArray());
			}

			using (var ms = new MemoryStream())
			{
				ImageBuilder.Current.Build(sourceImageBytes, ms, new ResizeSettings("maxwidth=500&maxheight=500&format=jpg&quality=80&watermark=logo"));
				listingPhoto.MediumSizeLength = ms.Length;
                BinaryStorage.StoreBytes(MediumSizeStoreId, listingPhoto.StoreItemID, ms.ToArray());
			}

			using (var ms = new MemoryStream())
			{
				ImageBuilder.Current.Build(sourceImageBytes, ms, new ResizeSettings("maxwidth=1200&maxheight=1200&format=jpg&scale=both&quality=80&watermark=bkg,logo,url,listingCode&listingCode=" + listing.Code));
				listingPhoto.FullSizeLength = ms.Length;
                BinaryStorage.StoreBytes(FullSizeStoreId, listingPhoto.StoreItemID, ms.ToArray());
			}
		}

		#endregion
	}
}