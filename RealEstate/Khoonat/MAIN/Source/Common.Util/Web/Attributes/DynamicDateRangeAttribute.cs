using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class DynamicDateRangeAttribute : ValidationAttribute
	{
		private readonly Func<DateTime?, bool> _maximum;
		private readonly Func<DateTime?, bool> _minimum;

		public DynamicDateRangeAttribute(string minimum, string maximum)
			: this()
		{
			// TODO: Implement string-based dynamic behavior (like "+1d", "-1w", ...)

			if (minimum != null) _minimum = delegate { throw new NotSupportedException("String-based dynamic date comparison is not implemented yet."); };
			if (maximum != null) _maximum = delegate { throw new NotSupportedException("String-based dynamic date comparison is not implemented yet."); };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
//		public DynamicDateRangeAttribute(int minimum = int.MinValue, int maximum = int.MaxValue)
//			: this()
//		{
//			if (minimum != int.MinValue) _minimum = dt => (DateTime.Now.Date.AddDays(minimum) <= dt);
//			if (maximum != int.MaxValue) _maximum = dt => (DateTime.Now.Date.AddDays(maximum) >= dt);
//		}
//
		public DynamicDateRangeAttribute(double minimum = double.NaN, double maximum = double.NaN)
			: this()
		{
			if (!double.IsNaN(minimum)) _minimum = dt => (DateTime.Now.Date.AddDays(minimum) <= dt);
			if (!double.IsNaN(maximum)) _maximum = dt => (DateTime.Now.Date.AddDays(maximum) >= dt);
		}

		private DynamicDateRangeAttribute()
			: base(() => "Specified date is not in the valid range.")
		{
			_minimum = dt => true;
			_maximum = dt => true;
		}

		public override bool IsValid(object value)
		{
			if (value == null)
				return true;

			var dtValue = value as DateTime?;
			if (!dtValue.HasValue)
				return true;

			return _minimum(dtValue) && _maximum(dtValue);
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, (object) name);
		}
	}
}