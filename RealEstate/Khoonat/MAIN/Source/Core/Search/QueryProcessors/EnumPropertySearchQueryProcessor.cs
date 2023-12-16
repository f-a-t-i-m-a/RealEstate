using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace JahanJooy.RealEstate.Core.Search.QueryProcessors
{
	public class EnumPropertySearchQueryProcessor : IPropertySearchQueryProcessor
	{
		private readonly PropertyInfo _property;
		private readonly string _abbreviation;
		private readonly bool _propertyIsNullable;
		private readonly Type _enumType;
		private readonly Type _enumUnderlyingType;

		public EnumPropertySearchQueryProcessor(PropertyInfo property, string abbreviation)
		{
			_property = property;
			_abbreviation = abbreviation;
			_propertyIsNullable = Nullable.GetUnderlyingType(property.PropertyType) != null;
			_enumType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
			
			if (!_enumType.IsEnum)
				throw new ArgumentException("Property " + property.Name + " is not an 'enum' type property.");

			_enumUnderlyingType = Enum.GetUnderlyingType(_enumType);
		}

		#region Implementation of IPropertySearchQueryProcessor

		public IEnumerable<string> GetParsableKeywords()
		{
			if (_enumUnderlyingType == typeof(byte))
				return Enum.GetValues(_enumType).Cast<byte>().Select(i => _abbreviation + i.ToString(CultureInfo.InvariantCulture));
			if (_enumUnderlyingType == typeof(int))
				return Enum.GetValues(_enumType).Cast<int>().Select(i => _abbreviation + i.ToString(CultureInfo.InvariantCulture));

			throw new ArgumentException("Not supported underlying enum type: " + _enumUnderlyingType.Name);
		}

		public int ParseQuerySegment(List<string> segments, int index, PropertySearch result)
		{
			try
			{
				string enumValueString = segments[index];
				if (!enumValueString.StartsWith(_abbreviation))
					return 0;

				enumValueString = enumValueString.Replace(_abbreviation, "");
				int enumValueNumber;
				if (!int.TryParse(enumValueString, out enumValueNumber))
					return 0;

				object enumValue = Enum.ToObject(_enumType, enumValueNumber);
				_property.SetValue(result, enumValue, null);
				return 1;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public bool GenerateQuerySegment(PropertySearch propertySearch, List<string> segments)
		{
			object enumValue = _property.GetValue(propertySearch, null);
			if (enumValue == null)
				return false;

			if (_enumUnderlyingType == typeof(byte))
				segments.Add(_abbreviation + ((byte)enumValue).ToString(CultureInfo.InvariantCulture));
			else if (_enumUnderlyingType == typeof(int))
				segments.Add(_abbreviation + ((int)enumValue).ToString(CultureInfo.InvariantCulture));
			else
				throw new ArgumentException("Not supported underlying enum type: " + _enumUnderlyingType.Name);

			return true;
		}

		#endregion
	}
}