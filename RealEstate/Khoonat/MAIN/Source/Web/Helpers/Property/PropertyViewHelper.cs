using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Util.Presentation;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Web.Helpers.Property
{
	public static class PropertyViewHelper
	{
		public static IHtmlString SimpleAttribute(bool showIfNull, string title, object value, string additionalStyle = null, string nullReplacement = null)
		{
			if (value == null && !showIfNull)
				return null;

			return Div((string.IsNullOrWhiteSpace(title) ? "" : ("<strong>" + AntiXssEncoder.HtmlEncode(title, false) + "</strong>: ")) +
			           HtmlPresentationHelper.IfNotSpecified(value, nullReplacement).ToHtmlString(), additionalStyle);
		}

		public static IHtmlString MeasuredAttribute(bool showIfNull, string title, string format, object value, string additionalStyle = null, string nullReplacement = null)
		{
			if (value == null && !showIfNull)
				return null;

			return Div((string.IsNullOrWhiteSpace(title) ? "" : ("<strong>" + AntiXssEncoder.HtmlEncode(title, false) + "</strong>: ")) +
			           HtmlPresentationHelper.IfNotSpecified(format.FormatIfNotNull(value), nullReplacement).ToHtmlString(), additionalStyle);
		}

		public static IHtmlString EnumAttribute<TEnum>(bool showIfNull, string title, TEnum? value, ResourceManager resourceManager = null, string additionalStyle = null)
			where TEnum : struct
		{
			if (!value.HasValue && !showIfNull)
				return null;

			return Div("<strong>" + AntiXssEncoder.HtmlEncode(title, false) + "</strong>: " +
			           HtmlPresentationHelper.IfNotSpecified(value.Label(resourceManager ?? DomainEnumResources.ResourceManager)), additionalStyle);
		}

		public static IHtmlString BooleanAttribute(UrlHelper url, bool showIfNull, string title, bool? value, bool showIcon = true, bool invertIconValue = false, string additionalStyle = null)
		{
			if (!value.HasValue && !showIfNull)
				return null;

			string result = "";
			title += ": ";

			if (showIcon)
				result += IconsHelper.Tristate(url, invertIconValue ? !value : value).ToHtmlString() + " ";

			result += value.HasValue
				          ? AntiXssEncoder.HtmlEncode(title, false)
				          : HtmlPresentationHelper.Disabled(title).ToHtmlString();

			result += value.HasValue
				          ? "<strong>" + (value.Value ? GeneralResources.Yes : GeneralResources.No) + "</strong>"
						  : HtmlPresentationHelper.Disabled(GeneralResources.Unknown).ToHtmlString();

			return Div(result, additionalStyle);
		}

		private static IHtmlString Div(string encodedContent, string additionalStyle = null)
		{
			return new HtmlString("<div class='col-lg-4 col-sm-6 col-xs-12'><div class='detail-item'>" + encodedContent + "</div></div>");
		}

		public static IHtmlString Container(params object[] items)
		{
			if (items == null || items.Length < 1)
				return null;

			var resultBuilder = new StringBuilder();

			foreach (var item in items)
			{
				if (item == null)
					continue;
				
				var itemHtmlString = item as IHtmlString;
				resultBuilder.Append(itemHtmlString != null ? itemHtmlString.ToHtmlString() : AntiXssEncoder.HtmlEncode(item.ToString(), false));
			}

			var result = resultBuilder.ToString();
			if (string.IsNullOrWhiteSpace(result))
				return null;

			return new HtmlString("<div class='row'>" + result + "</div>");
		}
	}
}