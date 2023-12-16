using System.Web;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Extensions
{
	public static class CheckBoxHtmlExtensions
	{
		public static IHtmlString CheckBoxWithoutHiddenField(this HtmlHelper htmlHelper, string name, bool isChecked)
		{
			var result = new TagBuilder("input");
			result.MergeAttribute("name", name);
			result.MergeAttribute("id", name);
			result.MergeAttribute("type", "checkbox");
			result.MergeAttribute("value", "true");
			if (isChecked)
				result.MergeAttribute("checked", "checked");

			return new HtmlString(result.ToString(TagRenderMode.SelfClosing));
		}
	}
}