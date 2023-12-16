using System.ComponentModel.DataAnnotations;

namespace JahanJooy.Common.Util.Web.Attributes
{
	/// <summary>
	/// Specifies that a property should have a date-formatted string value.
	/// Extends, and uses, regular expression validation method and specifies
	/// date format in a regular expression for validation.
	/// </summary>
	public class DateAttribute : RegularExpressionAttribute
	{
		public DateAttribute()
			: base(@"^(1[34][0-9][0-9])/(0?[1-9]|10|11|12)/(0?[1-9]|[12][0-9]|30|31)$")
		{

		}

		public override bool IsValid(object value)
		{
			return base.IsValid(value);
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			return base.IsValid(value, validationContext);
		}
	}
}