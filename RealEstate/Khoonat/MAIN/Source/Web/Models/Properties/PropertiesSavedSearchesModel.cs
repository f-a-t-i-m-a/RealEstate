using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSavedSearchesModel
	{
		public class SavedPropertySearchModel
		{
			public SavedPropertySearch SavedPropertySearch { get; set; }
			public PropertySearch PropertySearch { get; set; }
			public string[] FullSearchTitleParts { get; set; }
		}

		public List<SavedPropertySearchModel> SavedSearches { get; set; }

		public bool ShowCreditLowWarning { get; set; }
		public bool ShowNoEmailInProfileWarning { get; set; }
		public bool ShowNoPhoneNumberInProfileWarning { get; set; }
		public bool ShowSavedSearchWithoutContactWarning { get; set; }
	}
}