using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JahanJooy.RealEstate.Core.Search.QueryProcessors
{
	public class NumericPropertySearchQueryProcessor : IPropertySearchQueryProcessor
	{
		private readonly PropertyInfo _property;
		private readonly PropertyInfo[] _childProperties;
		private readonly string _abbreviation;
		private readonly Type _numericType;

		public NumericPropertySearchQueryProcessor(PropertyInfo property, string abbreviation, PropertyInfo[] childProperties = null)
		{
			_property = property;
			_childProperties = childProperties;
			_abbreviation = abbreviation;
			_numericType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

			if (_numericType != typeof(int) &&
				_numericType != typeof(long) &&
				_numericType != typeof(decimal))
			{
				throw new ArgumentException("Property " + property.Name + " is not a numeric type property.");
			}
		}

		#region Implementation of IPropertySearchQueryProcessor

		public IEnumerable<string> GetParsableKeywords()
		{
			return new [] { _abbreviation };
		}

		public int ParseQuerySegment(List<string> segments, int index, PropertySearch result)
		{
			if (!segments[index].Equals(_abbreviation, StringComparison.InvariantCultureIgnoreCase))
				return 0;

			if (segments.Count < index + 2)
				return 0;

			string numericValueString = segments[index + 1];
			object numericValue = null;
			bool parseSucceeded = false;

			if (_numericType == typeof(int))
			{
				int intValue;
				parseSucceeded = int.TryParse(numericValueString, out intValue);

				if (parseSucceeded)
					numericValue = intValue;
			}
			else if (_numericType == typeof(long))
			{
				long longValue;
				parseSucceeded = long.TryParse(numericValueString, out longValue);

				if (parseSucceeded)
					numericValue = longValue;
			}
			else if (_numericType == typeof(decimal))
			{
				decimal decimalValue;
				parseSucceeded = decimal.TryParse(numericValueString, out decimalValue);

				if (parseSucceeded)
					numericValue = decimalValue;
			}

			if (!parseSucceeded)
				return 0;

			_property.SetValue(result, numericValue, null);
			return 2;
		}

		public bool GenerateQuerySegment(PropertySearch propertySearch, List<string> segments)
		{
			if (_childProperties != null)
				if (_childProperties.Any(p => p.GetValue(propertySearch, null) != null))
					return false;

			object numericValue = _property.GetValue(propertySearch, null);
			if (numericValue == null)
				return false;

			segments.Add(_abbreviation);
			segments.Add(numericValue.ToString());
			return true;
		}

		#endregion
	}
}