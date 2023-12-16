using System.Web;
using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Helpers
{
	public static class TabHelper
	{
		public static HtmlString OutputTab(string title, string href, bool active)
		{
			// There's no need to HTML-Encode for XSS, because the TagBuilder class encodes the input appropriately by itself.

			var result = new TagBuilder("a");
			result.MergeAttribute("href", href);
			result.SetInnerText(title);

			if (active)
				result.MergeAttribute("class", "youarehere");

			return new HtmlString(result.ToString());
		}
	}
}