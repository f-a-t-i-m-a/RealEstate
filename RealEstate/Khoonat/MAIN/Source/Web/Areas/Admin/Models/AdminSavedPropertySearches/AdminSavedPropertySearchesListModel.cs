using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSavedPropertySearches
{
	public class AdminSavedPropertySearchesListModel
	{
		public class SavedPropertySearchAdminInfo
		{
			public SavedPropertySearch SavedSearch { get; set; }
			public string[] CriteriaTextParts { get; set; }
			public bool HasNotificationTargetError { get; set; }
		}

		public PagedList<SavedPropertySearchAdminInfo> SavedSearchInfos { get; set; }
		public string Page { get; set; }

		public long? IdFilter { get; set; }
		public long? UserIdFilter { get; set; }
		public long? CreatorSessionIdFilter { get; set; }
		public bool? DeletedFilter { get; set; }
		public SavedPropertySearchNotificationFilterType? NotificationFilter { get; set; }
		public string NotificationTargetFilter { get; set; }
		public bool NotificationTargetErrorFilter { get; set; }
	}

	public enum SavedPropertySearchNotificationFilterType
	{
		None,
		Email,
		PromotionalSms,
		PaidSms,
		AnySms,
		Any,
		All
	}
}