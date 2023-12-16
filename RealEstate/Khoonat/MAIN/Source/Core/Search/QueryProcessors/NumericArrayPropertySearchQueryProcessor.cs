using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JahanJooy.RealEstate.Core.Search.QueryProcessors
{
	public class NumericArrayPropertySearchQueryProcessor : IPropertySearchQueryProcessor
	{
		private readonly PropertyInfo _property;
		private readonly PropertyInfo[] _childProperties;
		private readonly string _abbreviation;
		private readonly Type _numericType;

		public NumericArrayPropertySearchQueryProcessor(PropertyInfo property, string abbreviation, PropertyInfo[] childProperties = null)
		{
			if (!property.PropertyType.IsArray)
				throw new ArgumentException("Property " + property.Name + " is not an array.");

			_property = property;
			_childProperties = childProperties;
			_abbreviation = abbreviation;
			_numericType = property.PropertyType.GetElementType();

			if (_numericType != typeof(int) &&
				_numericType != typeof(long) &&
				_numericType != typeof(decimal))
			{
				throw new ArgumentException("Property " + property.Name + " is not a numeric-type array.");
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

			var arrayValueString = segments[index + 1];
			var listValue = new List<object>();
			var parseSucceeded = true;

			foreach (var numericValueString in arrayValueString.Split('_'))
			{
				if (!parseSucceeded)
					break;

				if (_numericType == typeof(int))
				{
					int intValue;
					parseSucceeded = int.TryParse(numericValueString, out intValue);

					if (parseSucceeded)
						listValue.Add(intValue);
				}
				else if (_numericType == typeof(long))
				{
					long longValue;
					parseSucceeded = long.TryParse(numericValueString, out longValue);

					if (parseSucceeded)
						listValue.Add(longValue);
				}
				else if (_numericType == typeof(decimal))
				{
					decimal decimalValue;
					parseSucceeded = decimal.TryParse(numericValueString, out decimalValue);

					if (parseSucceeded)
						listValue.Add(decimalValue);
				}
				else
				{
					throw new ArgumentException("Unsupported type");
				}
			}

			if (!parseSucceeded)
				return 0;

			object arrayValue;
			if (_numericType == typeof(int))
				arrayValue = listValue.Cast<int>().ToArray();
			else if (_numericType == typeof(long))
				arrayValue = listValue.Cast<long>().ToArray();
			else if (_numericType == typeof (decimal))
				arrayValue = listValue.Cast<decimal>().ToArray();
			else
				throw new ArgumentException("Unsupported type");

			_property.SetValue(result, arrayValue, null);
			return 2;
		}

		public bool GenerateQuerySegment(PropertySearch propertySearch, List<string> segments)
		{
			if (_childProperties != null)
				if (_childProperties.Any(p => p.GetValue(propertySearch, null) != null))
					return false;

			var arrayValue = (Array)_property.GetValue(propertySearch, null);
			if (arrayValue == null)
				return false;

			var arrayValueString = string.Join("_", arrayValue.Cast<object>());
			segments.Add(_abbreviation);
			segments.Add(arrayValueString);
			return true;
		}

		#endregion
	}
}