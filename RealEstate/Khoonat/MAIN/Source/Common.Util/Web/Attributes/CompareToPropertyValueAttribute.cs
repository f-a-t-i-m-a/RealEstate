using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class CompareToPropertyValueAttribute : ValidationAttribute
	{
		private readonly string _propertyName;
		private readonly PropertyValidationComparisonUtil.ComparisonType _comparisonType;

		public CompareToPropertyValueAttribute(PropertyValidationComparisonUtil.ComparisonType comparisonType, string propertyName)
		{
			_propertyName = propertyName;
			_comparisonType = comparisonType;

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			PropertyInfo property = validationContext.ObjectType.GetProperty(_propertyName);
			if (property == null)
			{
				return new ValidationResult("PropertyName is not specified.");
			}

			object targetValue = property.GetValue(validationContext.ObjectInstance, null);

			var comparisonResult = PropertyValidationComparisonUtil.Compare(value, _comparisonType, targetValue);
			if (!comparisonResult.HasValue || comparisonResult.Value)
				return null;

			return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, (object)name);
		}
	}
}