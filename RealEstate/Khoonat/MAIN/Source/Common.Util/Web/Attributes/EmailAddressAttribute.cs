using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using JahanJooy.Common.Util.Text;

namespace JahanJooy.Common.Util.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EmailAddressAttribute : ValidationAttribute
    {
        public EmailAddressAttribute(bool allowPartialMatch = false)
        {
            AllowPartialMatch = allowPartialMatch;
        }

        public EmailAddressAttribute(string errorMessage, bool allowPartialMatch = false) : base(errorMessage)
        {
            AllowPartialMatch = allowPartialMatch;
        }

        public EmailAddressAttribute(Func<string> errorMessageAccessor, bool allowPartialMatch = false) : base(errorMessageAccessor)
        {
            AllowPartialMatch = allowPartialMatch;
        }

        private bool AllowPartialMatch { get; set; }

		public override bool IsValid(object value)
		{
			string input = Convert.ToString(value, CultureInfo.CurrentCulture);

			if (string.IsNullOrEmpty(input))
				return true;

		    return EmailUtils.IsValidEmail(input, AllowPartialMatch);
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name });
		}
    }
}