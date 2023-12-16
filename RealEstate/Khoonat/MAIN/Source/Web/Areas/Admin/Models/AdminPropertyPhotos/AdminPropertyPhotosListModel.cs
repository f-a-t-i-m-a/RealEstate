using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminPropertyPhotos
{
	public class AdminPropertyPhotosListModel
	{
		public PagedList<PropertyListingPhoto> Photos { get; set; }
		public string Page { get; set; }

		public long? IdFilter { get; set; }
		public long? ListingIdFilter { get; set; }
		public long? SessionIdFilter { get; set; }
		public bool ApplyUserIdFilter { get; set; }
		public long? UserIdFilter { get; set; }
		public bool ApplyOwnerUserIdFilter { get; set; }
		public long? OwnerUserIdFilter { get; set; }

		public PropertyListingPhotoApprovalStatus? ApprovalStatusFilter { get; set; }
		public bool? DeletedFilter { get; set; }
	}

	public enum PropertyListingPhotoApprovalStatus
	{
		NotApproved,
		Approved,
		Rejected
	}
}