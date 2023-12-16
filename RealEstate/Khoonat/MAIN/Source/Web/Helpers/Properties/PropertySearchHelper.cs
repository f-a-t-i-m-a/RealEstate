using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Util;
using JahanJooy.RealEstate.Util.Presentation;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Models.Properties;
using JahanJooy.RealEstate.Web.Resources.Helpers.Properties;
using JahanJooy.RealEstate.Web.Resources.Views.Properties;

namespace JahanJooy.RealEstate.Web.Helpers.Properties
{
	[Contract]
	[Component]
	public class PropertySearchHelper
	{
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public VicinityPresentationHelper VicinityPresentationHelper { get; set; }

		public string GetShortTitle(PropertySearch search)
		{
			var intentionText = search.IntentionOfOwner.Label(PropertySearchTitleResources.ResourceManager) ?? PropertySearchTitleResources.IntentionOfOwner_All;
			var propertyTypeText = search.PropertyType.Label(PropertySearchTitleResources.ResourceManager) ?? PropertySearchTitleResources.PropertyType_All;

			string locationText = BuildLocationText(search);

			string result = string.IsNullOrWhiteSpace(intentionText) ? propertyTypeText : intentionText + " " + propertyTypeText;
			if (!string.IsNullOrWhiteSpace(locationText))
				result += " " + string.Format(PropertySearchTitleResources.LocationTitleFormat, locationText);

			return result;
		}

		public string GetFullText(PropertySearch search)
		{
			var result = string.Join(GeneralResources.Semicolon, GetFullTextParts(search));
			return string.IsNullOrWhiteSpace(result) ? PropertySearchTitleResources.AllProperties : result;
		}

		public string[] GetFullTextParts(PropertySearch search)
		{
			var options = ExtractSelectedOptions(search);
			if (options == null || !options.Any())
				return new string[0];

			return options.Select(option => string.Format(PropertiesBrowseResources.ResourceManager.GetString(option.Label) ?? option.Label, option.LabelParam)).ToArray();
		}

		public string BuildLocationText(PropertySearch search)
		{
			return BuildLocationText(search.VicinityIDs);
		}

		public IEnumerable<PropertiesSearchOptionModel> ExtractSelectedOptions(PropertySearch search, bool generateQueries = true)
		{
			var selectedOptions = new List<PropertiesSearchOptionModel>();

			if (search == null)
				return selectedOptions;

			var tempSearch = PropertySearch.Copy(search);

			FillSelectedOptionsOfGeneralProperties(tempSearch, selectedOptions, generateQueries);
			FillSelectedOptionsOfProperties(tempSearch, selectedOptions, generateQueries);
			FillSelectedOptionsOfStringList(tempSearch, tempSearch.Options, PropertySearchOptions.AllOptions, selectedOptions, generateQueries);

			return selectedOptions;
		}

		private string BuildLocationText(long[] vicinityIDs, bool allVicinities = false)
		{
			string locationText = null;


            if (vicinityIDs != null && vicinityIDs.Length == 1 && VicinityCache[vicinityIDs[0]] != null)
            {
               locationText = VicinityDisplayItem.ToString(
							 VicinityPresentationHelper.BuildHierarchyString(VicinityCache[vicinityIDs[0]].ID,
                                 useWellknownScope: true, summary: true));
            }
            else if (vicinityIDs != null && vicinityIDs.Length > 1 && !allVicinities)
            {
                locationText = string.Format(PropertySearchTitleResources.MultipleVicinityNames, vicinityIDs.Length);
            }
            else if (vicinityIDs != null && vicinityIDs.Length > 1 && allVicinities)
            {
                var vicinityNames = string.Join(GeneralResources.Comma, vicinityIDs.Where(vid => VicinityCache[vid] != null).Select(vid => VicinityCache[vid].Name));      
                locationText = string.Format(PropertySearchTitleResources.MultipleVicinityNames, vicinityNames);
            }
			return locationText;
		}

		private void FillSelectedOptionsOfGeneralProperties(PropertySearch search, List<PropertiesSearchOptionModel> selectedOptions, bool generateQueries)
		{
			if (search.PropertyType.HasValue)
			{
				var selectedOption = new PropertiesSearchOptionModel
				                     {
					                     Label = "PropertyType",
					                     LabelParam = search.PropertyType.Label(DomainEnumResources.ResourceManager),
										 QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(search)
				                     };

				if (generateQueries)
				{
					var originalValue = search.PropertyType;
					search.PropertyType = null;
					selectedOption.QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(search);
					search.PropertyType = originalValue;
				}

				selectedOptions.Add(selectedOption);
			}

			if (search.IntentionOfOwner.HasValue)
			{
				var selectedOption = new PropertiesSearchOptionModel
				{
					Label = "IntentionOfOwner",
					LabelParam = search.IntentionOfOwner.Label(DomainEnumResources.ResourceManager)
				};

				if (generateQueries)
				{
					var originalValue = search.IntentionOfOwner;
					search.IntentionOfOwner = null;
					selectedOption.QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(search);
					search.IntentionOfOwner = originalValue;
				}

				selectedOptions.Add(selectedOption);
			}

			if (search.VicinityIDs != null && search.VicinityIDs.Any())
		    {
				var originalVicinityIDs = search.VicinityIDs.ToList();

				for (var i = 0; i < originalVicinityIDs.Count; i++)
		        {
                    long currentVicinity = originalVicinityIDs[i];
		            var selectedOption = new PropertiesSearchOptionModel();
		            selectedOption.Label = originalVicinityIDs.Count > 1 ? "Vicinities" : "Vicinity";

		            var vicinity = VicinityCache[originalVicinityIDs[i]];

		            selectedOption.LabelParam = vicinity.IfNotNull(
		                v => VicinityPresentationHelper.BuildTitle(v),
		                GeneralResources.Unknown);

		            if (generateQueries)
		            {
                        originalVicinityIDs.RemoveAt(i);
		                search.VicinityIDs = originalVicinityIDs.ToArray();
		                selectedOption.QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(search);
                        originalVicinityIDs.Insert(i, currentVicinity);
		                search.VicinityIDs = originalVicinityIDs.ToArray();
		            }

		            selectedOptions.Add(selectedOption);
		        }
		    }
		}

		private static void FillSelectedOptionsOfProperties(PropertySearch tempSearch, List<PropertiesSearchOptionModel> selectedOptions, bool generateQueries)
		{
			var propertyNames = new[] { 
				"UnitAreaMinimum", "UnitAreaMaximum", "EstateAreaMinimum", "EstateAreaMaximum", "NumberOfRoomsMinimum", "NumberOfRoomsMaximum", "NumberOfParkingsMinimum",
				"SalePriceMinimum", "SalePriceMaximum", "SalePricePerEstateAreaMinimum", "SalePricePerEstateAreaMaximum", "SalePricePerUnitAreaMinimum", "SalePricePerUnitAreaMaximum", 
				"RentMortgageMinimum", "RentMortgageMaximum", "RentMinimum", "RentMaximum"
			};

			foreach (string propertyName in propertyNames)
			{
				var propertyInfo = typeof(PropertySearch).GetProperty(propertyName);
				var currentValue = propertyInfo.GetValue(tempSearch, null);
				if (currentValue == null)
					continue;

				var selectedOption = new PropertiesSearchOptionModel
				                                  {
					                                  Label = "property_" + propertyInfo.Name,
					                                  LabelParam = currentValue,
				                                  };

				if (generateQueries)
				{
					propertyInfo.SetValue(tempSearch, null, null);
					selectedOption.QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(tempSearch);
					propertyInfo.SetValue(tempSearch, currentValue, null);
				}

				selectedOptions.Add(selectedOption);
			}
		}

		private static void FillSelectedOptionsOfStringList(PropertySearch tempSearch, List<string> stringList, IDictionary<string, PropertySearchOption> referenceDic, List<PropertiesSearchOptionModel> selectedOptions, bool generateQueries)
		{
			if (stringList == null)
				return;

			for (var i = 0; i < stringList.Count; i++)
			{
				string currentOptionKey = stringList[i];
				PropertySearchOption currentOption = referenceDic[currentOptionKey];

				var selectedOption = new PropertiesSearchOptionModel
				                     {
					                     Label = "option_" + currentOption.Label.ToLower(),
					                     LabelParam = null,
				                     };

				if (generateQueries)
				{
					stringList.RemoveAt(i);
					selectedOption.QueryAfterRemoval = PropertySearchQueryUtil.GenerateQuery(tempSearch);
					stringList.Insert(i, currentOptionKey);
				}

				selectedOptions.Add(selectedOption);
			}
		}
	}
}