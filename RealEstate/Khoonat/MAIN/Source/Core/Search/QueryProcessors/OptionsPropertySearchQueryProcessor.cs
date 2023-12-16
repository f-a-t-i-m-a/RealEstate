using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstate.Core.Search.QueryProcessors
{
	public class OptionsPropertySearchQueryProcessor : IPropertySearchQueryProcessor
	{
		private readonly PropertyInfo _property;
		private readonly HashSet<string> _possibleValues;

		public OptionsPropertySearchQueryProcessor(PropertyInfo property, IEnumerable<string> possibleValues)
		{
			_property = property;
			_possibleValues = new HashSet<string>(possibleValues.Select(s => s.ToLower()));
			
			if (_property.PropertyType != typeof(List<string>))
				throw new ArgumentException("Property " + property.Name + " is not a list of strings.");
		}

		#region Implementation of IPropertySearchQueryProcessor

		public IEnumerable<string> GetParsableKeywords()
		{
			return _possibleValues;
		}

		public int ParseQuerySegment(List<string> segments, int index, PropertySearch result)
		{
			string keyword = segments[index].ToLower();
			if (!_possibleValues.Contains(keyword))
				return 0;

			var optionList = (List<string>)_property.GetValue(result, null);
			if (optionList == null)
			{
				optionList = new List<string>();
				_property.SetValue(result, optionList, null);
			}

			optionList.AddIfNotContains(keyword);
			return 1;
		}

		public bool GenerateQuerySegment(PropertySearch propertySearch, List<string> segments)
		{
			var optionList = (List<string>)_property.GetValue(propertySearch, null);
			if (optionList == null || optionList.Count == 0)
				return false;

			segments.AddRange(optionList);

			return true;
		}

		#endregion
	}
}