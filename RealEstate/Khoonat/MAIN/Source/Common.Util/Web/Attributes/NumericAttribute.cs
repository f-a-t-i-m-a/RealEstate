using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JahanJooy.Common.Util.Web.Attributes
{
	/// <summary>
	/// Specifies that a property should have a numeric value.
	/// This is only used to provide a localized message for the client-side
	/// validation using LocalizedClientDataTypeModelValidatorProvider class.
	/// The actual server-side validation is performed in the model binder,
	/// based on the type of the target property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class NumericAttribute : ValidationAttribute
	{
//		private static string CreatePattern(bool allowDecimal)
//		{
//			if (allowDecimal) 
//				return "\\d*(\\.?\\d+)?";
//
//			return "\\d*";
//		}

		public override bool IsValid(object value)
		{
			// No extra validation required.
			// The binder will set an error if the numeric value can not be parsed.

			return true;
		}

		public override string FormatErrorMessage(string name)
		{
			return GetErrorMessage();
		}

		public string GetErrorMessage()
		{
			if (string.IsNullOrEmpty(ErrorMessage))
			{
				PropertyInfo prop = ErrorMessageResourceType.GetProperty(ErrorMessageResourceName);
				return prop.GetValue(null, null).ToString();
			}

			return ErrorMessage;
		}
	}
}