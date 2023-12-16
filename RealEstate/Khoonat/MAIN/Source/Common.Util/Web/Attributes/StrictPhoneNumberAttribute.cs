using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using JahanJooy.Common.Util.Localization;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class StrictPhoneNumberAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			string input = Convert.ToString(value, CultureInfo.CurrentCulture);
			if (string.IsNullOrWhiteSpace(input))
				return true;

			var parsedNumber = LocalPhoneNumberUtils.ParseAndValidate(input);
			return parsedNumber != null;
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name });
		}
	}
}