using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security.AntiXss;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util.Presentation
{
	public class HtmlPresentationHelper
	{
		private static readonly Regex WhitespacePattern = new Regex(@"\s+");
		private static readonly Regex HtmlWhitespacePattern = new Regex(@"(\s|&nbsp;)*");

		public static HtmlString Disabled(object value)
		{
			return new HtmlString("<span class=\"grayed\">" + (value is HtmlString ? value.ToString() : AntiXssEncoder.HtmlEncode(value.ToString(), false)) + "</span>");
		}

		public static HtmlString IfNotSpecified(object value, object replacement = null)
		{
			if (value == null)
				return Disabled(replacement ?? GeneralResources.NotSpecified);

			var valueString = value is HtmlString ? value.ToString() : AntiXssEncoder.HtmlEncode(value.ToString(), false);

			if (String.IsNullOrWhiteSpace(valueString))
				return Disabled(replacement ?? GeneralResources.NotSpecified);

			return new HtmlString(valueString);
		}

		public static HtmlString IfNotNull(object value, object replacement = null)
		{
			if (value == null)
				return Disabled(replacement ?? GeneralResources.NotSpecified);

			var valueString = value is HtmlString ? value.ToString() : AntiXssEncoder.HtmlEncode(value.ToString(), false);
			return new HtmlString(valueString);
		}

		public static HtmlString NoBreaking(object input)
		{
			if (input == null)
				return null;

			var inputString = input is HtmlString ? input.ToString() : AntiXssEncoder.HtmlEncode(input.ToString(), false);

			return new HtmlString(WhitespacePattern.Replace(inputString, "&nbsp;"));
		}

		public static bool IsNullOrWhiteSpace(string input)
		{
			return HtmlWhitespacePattern.IsMatch(input);
		}

		public static HtmlString LeftToRight(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
				return null;

			return new HtmlString("<span style=\"direction: ltr;\">&lrm;" + AntiXssEncoder.HtmlEncode(input, false) + "</span>");
		}
	}
}
