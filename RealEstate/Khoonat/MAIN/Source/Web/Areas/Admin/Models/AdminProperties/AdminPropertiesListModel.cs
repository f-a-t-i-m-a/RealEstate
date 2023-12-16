using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminProperties
{
	public class AdminPropertiesListModel
	{
		public PagedList<PropertyListingSummary> PropertyListings { get; set; }
		public string Page { get; set; }

		public long? PropertyListingIdFilter { get; set; }
		public long? PropertyListingCodeFilter { get; set; }
		public bool ApplyOwnerUserIdFilter { get; set; }
		public long? OwnerUserIdFilter { get; set; }

		public PropertyListingPublishStatus? PublishStatusFilter { get; set; }
		public PropertyListingApprovalStatus? ApprovalStatusFilter { get; set; }
		public bool? DeletedFilter { get; set; }

        public string ConnectionInfoFilter { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationTypeFilter { get; set; }
	}

	public enum PropertyListingPublishStatus
	{
		NeverPublished,
		PublishedAndCurrent,
		PublishedAndPassed
	}

	public enum PropertyListingApprovalStatus
	{
		NotApproved,
		Approved,
		Rejected
	}
}