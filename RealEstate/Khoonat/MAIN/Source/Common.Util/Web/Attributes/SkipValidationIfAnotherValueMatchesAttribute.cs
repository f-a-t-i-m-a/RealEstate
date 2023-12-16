using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public class SkipValidationIfAnotherValueMatchesAttribute : SkipValidationIfAttribute
	{
		public string ValueProviderKey { get; set; }
		public string RegexPattern { get; set; }
		public MatchBehavior Behavior { get; set; }

		private Regex _regex;

		public SkipValidationIfAnotherValueMatchesAttribute()
		{
			ValueProviderKey = null;
			RegexPattern = null;
			Behavior = MatchBehavior.SkipIfWholeInputMatches;
		}

		#region Overrides of SkipValidationIfAttribute

		public override bool ShouldSkipValidation(ControllerContext controllerContext, ModelBindingContext bindingContext, ModelMetadata propertyMetadata, ModelValidationResult validationResult)
		{
			SetupRegex();

			if (string.IsNullOrEmpty(ValueProviderKey))
				throw new InvalidOperationException("ValueProviderKey can not be null or empty.");

			string input = bindingContext.ValueProvider.GetValue(ValueProviderKey).AttemptedValue;

			Match match = _regex.Match(input);

			switch (Behavior)
			{
				case MatchBehavior.SkipIfPartOfInputDoesntMatch:
					return !match.Success;

				case MatchBehavior.SkipIfPartOfInputMatches:
					return match.Success;

				case MatchBehavior.SkipIfWholeInputDoesntMatch:
					return (!match.Success) || (match.Index != 0) || (match.Length != input.Length);

				case MatchBehavior.SkipIfWholeInputMatches:
					return (match.Success) && (match.Index == 0) && (match.Length == input.Length);
			}

			return false;
		}

		#endregion

		private void SetupRegex()
		{
			if (_regex != null)
				return;

			if (string.IsNullOrEmpty(RegexPattern))
				throw new InvalidOperationException("RegexPattern can not be null or empty.");

			_regex = new Regex(RegexPattern);
		}
	}

	public enum MatchBehavior
	{
		SkipIfWholeInputMatches,
		SkipIfPartOfInputMatches,
		SkipIfWholeInputDoesntMatch,
		SkipIfPartOfInputDoesntMatch
	}
}