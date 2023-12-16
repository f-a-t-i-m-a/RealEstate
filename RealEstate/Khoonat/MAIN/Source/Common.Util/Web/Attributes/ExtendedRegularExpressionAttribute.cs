using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Text.RegularExpressions;

namespace JahanJooy.Common.Util.Web.Attributes
{
	/// <summary>
	/// Modified version of ResularExpressionAttribute from MVC3 framework.
	/// Optionally allows partial matches, and private members are changed to
	/// protected to allow usage in inheritance hierarchy.
	/// 
	/// Determining if a regular expression match is valid or not is also overridable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class ExtendedRegularExpressionAttribute : ValidationAttribute
	{
		public ExtendedRegularExpressionAttribute(string pattern)
			: base((() => "Value does not match to the expected pattern."))
		{
			Pattern = pattern;
		}

		public ExtendedRegularExpressionAttribute(string pattern, bool allowPartialMatch)
			: this(pattern)
		{
			AllowPartialMatch = allowPartialMatch;
		}

		public string Pattern { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		get; private set; }

		private Regex Regex { get; set; }
		private bool AllowPartialMatch { get; set; }

		public override bool IsValid(object value)
		{
			SetupRegex();
			string input = Convert.ToString(value, CultureInfo.CurrentCulture);

			if (string.IsNullOrEmpty(input))
				return true;

			Match match = Regex.Match(input);
			return match.Success && IsValid(match, input);
		}

		public override string FormatErrorMessage(string name)
		{
			SetupRegex();
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] {
				name,
				Pattern
			});
		}

		protected virtual bool IsValid(Match match, string input)
		{
			if (AllowPartialMatch)
				return true;

			return match.Index == 0 && match.Length == input.Length;
		}

		protected void SetupRegex()
		{
			if (Regex != null)
				return;
			if (string.IsNullOrEmpty(Pattern))
				throw new InvalidOperationException("Regular expression cannot be empty.");

			Regex = new Regex(Pattern);
		}
	}
}