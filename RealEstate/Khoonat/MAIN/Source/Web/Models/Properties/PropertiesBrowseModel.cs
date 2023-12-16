using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesBrowseModel
	{
        public class SavedPropertySearchModel
        {
            public SavedPropertySearch SavedPropertySearch { get; set; }
            public PropertySearch PropertySearch { get; set; }
        }

        public List<SavedPropertySearchModel> SavedSearches { get; set; }

        public PropertySearch Search { get; set; }
		public PropertySearch SeoCanonicalSearch { get; set; }
		public int RequestedPageNumber { get; set; }
		public int TotalNumberOfPages { get; set; }
		public SearchResult SearchResult { get; set; }

		public IEnumerable<PropertiesSearchMenuModel> BreadcrumbItems { get; set; }
		public IEnumerable<PropertiesSearchMenuModel> SortMenuItems { get; set; }
		public IEnumerable<PropertiesSearchMenuModel> PropertyTypeMenuItems { get; set; }
		public IEnumerable<PropertiesSearchMenuModel> IntentionMenuItems { get; set; }
		public IEnumerable<PropertiesSearchMenuModel> LocationMenuItems { get; set; }

		public string LocationText { get; set; }
		public IEnumerable<PropertiesSearchOptionModel> SelectedOptions { get; set; }
	}
}