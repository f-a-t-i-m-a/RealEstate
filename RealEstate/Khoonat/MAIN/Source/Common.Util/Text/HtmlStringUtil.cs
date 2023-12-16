using System.Linq;
using System.Web;
using System.Web.Security.AntiXss;

namespace JahanJooy.Common.Util.Text
{
	public class HtmlStringUtil
	{
		public static IHtmlString JoinNonEmpty(string separator, params object[] strings)
		{
			if (strings == null)
				return null;

			var values = strings
				.Where(s => s != null)
				.Select(s => s is HtmlString ? ((HtmlString) s).ToHtmlString() : AntiXssEncoder.HtmlEncode(s.ToString(), false))
				.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

			if (values.Length == 0)
				return null;

			return new HtmlString(
				string.Join(separator,
				            values));
		}
	}
}