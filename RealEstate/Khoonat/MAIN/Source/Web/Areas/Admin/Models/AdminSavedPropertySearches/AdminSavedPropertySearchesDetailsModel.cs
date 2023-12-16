using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSavedPropertySearches
{
	public class AdminSavedPropertySearchesDetailsModel
	{
		public SavedPropertySearch SavedSearch { get; set; }
		public string[] CriteriaTextParts { get; set; }
		public bool HasNotificationTargetError { get; set; }
	}
}