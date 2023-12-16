using System;
using System.Globalization;

namespace JahanJooy.Common.Util.Web.Attributes
{
	public class PropertyValidationComparisonUtil
	{
		public enum ComparisonType
		{
			Equals,
			NotEquals,
			LessThan,
			LessThanOrEquals,
			GreaterThan,
			GreaterThanOrEquals
		}

		public static bool? Compare(object firstValue, ComparisonType comparisonType, object secondValue)
		{
			switch (comparisonType)
			{
				case ComparisonType.Equals:
					if (secondValue == null)
						return firstValue == null;

					return secondValue.Equals(firstValue);

				case ComparisonType.NotEquals:
					if (secondValue == null)
						return firstValue != null;

					return !secondValue.Equals(firstValue);

				case ComparisonType.GreaterThan:
				case ComparisonType.GreaterThanOrEquals:
				case ComparisonType.LessThan:
				case ComparisonType.LessThanOrEquals:
					double? firstNumericValue = GetNumericValue(firstValue);
					double? secondNumericValue = GetNumericValue(secondValue);

					if (!firstNumericValue.HasValue || !secondNumericValue.HasValue)
						return null;

					return Compare(firstNumericValue.Value, comparisonType, secondNumericValue.Value);
			}

			return null;
		}

		public static bool? Compare(double firstValue, ComparisonType comparisonType, double secondValue)
		{
			switch (comparisonType)
			{
				case ComparisonType.GreaterThan:
					return firstValue > secondValue;

				case ComparisonType.GreaterThanOrEquals:
					return firstValue >= secondValue;

				case ComparisonType.LessThan:
					return firstValue < secondValue;

				case ComparisonType.LessThanOrEquals:
					return firstValue <= secondValue;
			}

			return null;
		}

		public static double? GetNumericValue(object value)
		{
			if (value == null)
				return null;

			if (value is Enum)
				return Convert.ToInt64(value);

			double result;
			if (Double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result))
				return result;

			return null;
		}
	}
}