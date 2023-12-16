using System;
using System.IO;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IPropertyPhotoService
	{
        PropertyListingPhoto AddPhoto(long listingId, Stream contents);
		ValidationResult UpdatePhotoProperties(PropertyListingPhoto photo);
		void DeletePhoto(long photoId);
		void SetApproved(long photoId, bool? approved);
		void RebuildPhoto(long photoId);

		byte[] GetThumbnailBytes(Guid storeItemId);
		byte[] GetMediumSizeBytes(Guid storeItemId);
		byte[] GetFullSizeBytes(Guid storeItemId);
		byte[] GetUntouchedBytes(Guid storeItemId);
	}
}