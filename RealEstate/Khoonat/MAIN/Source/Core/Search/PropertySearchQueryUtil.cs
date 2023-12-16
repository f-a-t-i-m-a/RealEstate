using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstate.Core.Search.QueryProcessors;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Search
{
	public static class PropertySearchQueryUtil
	{
		private static List<IPropertySearchQueryProcessor> _processors;
		private static Dictionary<string, List<IPropertySearchQueryProcessor>> _processorsDictionary;
		private static bool _initialized;
		
		public static PropertySearch ParseQuery(string query)
		{
			EnsureInitialized();

			var result = new PropertySearch();
			result.OriginalQuery = query;
			result.SortOrder = PropertySearchSortOrder.NewestFirst;

			if (string.IsNullOrWhiteSpace(query))
				return result;

			var segments = query.ToLower().Split('-').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
			for (int i = 0; i < segments.Count; )
			{
				string keyword = segments[i];
				if (!_processorsDictionary.ContainsKey(keyword))
					return null;

				int increment = 0;
				foreach (var processor in _processorsDictionary[keyword])
				{
					increment = processor.ParseQuerySegment(segments, i, result);
					if (increment > 0)
						break;
				}

				// If no match is found, ignore the segment

				if (increment > 0)
					i += increment;
				else
					i += 1;
			}

			result.FixLogicalErrors();
			result.Query = GenerateQuery(result);
			return result;
		}

		public static string GenerateQuery(PropertySearch propertySearch)
		{
			EnsureInitialized();

			if (propertySearch == null)
				return "";

			var resultSegments = new List<string>();

			foreach (var processor in _processors)
			{
				processor.GenerateQuerySegment(propertySearch, resultSegments);
			}

			return string.Join("-", resultSegments.Select(s => s.ToLower()));
		}

        public static string QueryForVicinity(long vicinityId)
		{
            return "vid-" + vicinityId;
		}
		private static void EnsureInitialized()
		{
			if (_initialized)
				return;

			lock (typeof(PropertySearchQueryUtil))
			{
				if (_initialized)
					return;

				Initialize();
				_initialized = true;
			}
		}

		private static void Initialize()
		{
			_processors = new List<IPropertySearchQueryProcessor>();
			_processorsDictionary = new Dictionary<string, List<IPropertySearchQueryProcessor>>();

			_processors.Add(new EnumPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SortOrder"), "srt"));
			_processors.Add(new EnumPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("PropertyType"), "ppt"));
			_processors.Add(new EnumPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("IntentionOfOwner"), "ioo"));

            _processors.Add(new NumericArrayPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("VicinityIDs"), "vid"));

			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("EstateAreaMinimum"), "min.ea"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("EstateAreaMaximum"), "max.ea"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("NumberOfRoomsMinimum"), "min.rm"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("NumberOfRoomsMaximum"), "max.rm"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("NumberOfParkingsMinimum"), "min.pk"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("UnitAreaMinimum"), "min.ua"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("UnitAreaMaximum"), "max.ua"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePriceMinimum"), "min.sp"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePriceMaximum"), "max.sp"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePricePerEstateAreaMinimum"), "min.sppea"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePricePerEstateAreaMaximum"), "max.sppea"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePricePerUnitAreaMinimum"), "min.sppua"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("SalePricePerUnitAreaMaximum"), "max.sppua"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("RentMortgageMinimum"), "min.mtg"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("RentMortgageMaximum"), "max.mtg"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("RentMinimum"), "min.rnt"));
			_processors.Add(new NumericPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("RentMaximum"), "max.rnt"));

			_processors.Add(new OptionsPropertySearchQueryProcessor(typeof(PropertySearch).GetProperty("Options"), PropertySearchOptions.AllOptions.Keys));

			foreach (var processor in _processors)
			{
				foreach (var keyword in processor.GetParsableKeywords().Select(s => s.ToLower()))
				{
					if (!_processorsDictionary.ContainsKey(keyword))
						_processorsDictionary[keyword] = new List<IPropertySearchQueryProcessor>();

					_processorsDictionary[keyword].Add(processor);
				}
			}
		}
	}
}