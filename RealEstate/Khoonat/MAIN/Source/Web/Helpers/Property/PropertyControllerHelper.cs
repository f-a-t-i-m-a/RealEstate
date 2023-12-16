using System.Web;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Session;

namespace JahanJooy.RealEstate.Web.Helpers.Property
{
	public static class PropertyControllerHelper
	{
		#region Private helper properties

		private static CorePrincipal User => HttpContext.Current == null ? CorePrincipal.Anonymous : ((HttpContext.Current.User as CorePrincipal) ?? CorePrincipal.Anonymous);

	    private static SessionInfo SessionInfo => HttpContext.Current.Session.GetSessionInfo();

	    #endregion

		public static bool DetermineIfUserIsOwner(PropertyListingDetails propertyListing, string providedEditPassword)
		{
			if (propertyListing == null)
				return false;

			return DetermineIfUserIsOwner(propertyListing.ID, propertyListing.OwnerUserID) || ValidateEditPassword(propertyListing, providedEditPassword);
		}

		public static bool DetermineIfUserIsOwner(PropertyListing propertyListing, string providedEditPassword)
		{
			if (propertyListing == null)
				return false;

			return DetermineIfUserIsOwner(propertyListing.ID, propertyListing.OwnerUserID) || ValidateEditPassword(propertyListing, providedEditPassword);
		}

		public static bool DetermineIfUserIsOwner(PropertyListingDetails propertyListing)
		{
			return propertyListing != null && DetermineIfUserIsOwner(propertyListing.ID, propertyListing.OwnerUserID);
		}

		public static bool DetermineIfUserIsOwner(PropertyListing propertyListing)
		{
			return propertyListing != null && DetermineIfUserIsOwner(propertyListing.ID, propertyListing.OwnerUserID);
		}

		public static bool DetermineIfUserIsOwner(PropertyListingSummary propertyListing)
		{
			return propertyListing != null && DetermineIfUserIsOwner(propertyListing.ID, propertyListing.OwnerUserID);
		}

		public static bool DetermineIfUserIsOwner(long listingID, long? ownerUserID)
		{
			if (SessionInfo?.OwnedProperties != null && SessionInfo.OwnedProperties.Contains(listingID))
				return true;

			if (User.Identity.IsAuthenticated && ownerUserID.HasValue && ownerUserID.Value == User.CoreIdentity.UserId)
				return true;

			return false;
		}

		public static bool ValidateEditPassword(PropertyListing propertyListing, string providedEditPassword)
		{
			return propertyListing != null && providedEditPassword != null && propertyListing.EditPassword == providedEditPassword;
		}

		public static bool ValidateEditPassword(PropertyListingDetails propertyListing, string providedEditPassword)
		{
			return propertyListing != null && providedEditPassword != null && propertyListing.EditPassword == providedEditPassword;
		}

		public static bool CanViewListing(PropertyListingDetails listing)
		{
			return listing != null && (listing.IsPublic() || DetermineIfUserIsOwner(listing) || User.IsOperator);
		}

        public static bool CanViewListing(PropertyListingSummary listing)
        {
            return listing != null && (listing.IsPublic() || DetermineIfUserIsOwner(listing) || User.IsOperator);
        }

		public static bool CanViewListing(PropertyListing listing)
		{
			return listing != null && (listing.IsPublic() || DetermineIfUserIsOwner(listing) || User.IsOperator);
		}

		public static bool CanViewListingPhoto(PropertyListingPhoto photo)
		{
			if (photo?.PropertyListing == null)
				return false;

			if (User.IsOperator || DetermineIfUserIsOwner(photo.PropertyListing))
				return true;

			return photo.PropertyListing.IsPublic() && photo.Approved.HasValue && photo.Approved.Value;
		}

		public static bool CanEditListingPhoto(PropertyListingPhoto photo)
		{
			if (photo?.PropertyListing == null)
				return false;

			return User.IsOperator || DetermineIfUserIsOwner(photo.PropertyListing);
		}

		public static bool CanEditListing(PropertyListingDetails propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}

		public static bool CanEditListing(PropertyListing propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}

		public static bool CanEditListing(PropertyListingSummary propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}

		public static bool CanPublishListing(PropertyListingDetails propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}

		public static bool CanPublishListing(PropertyListing propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}

		public static bool CanPublishListing(PropertyListingSummary propertyListing)
		{
			return propertyListing != null && (DetermineIfUserIsOwner(propertyListing) || User.IsOperator);
		}
	}
}