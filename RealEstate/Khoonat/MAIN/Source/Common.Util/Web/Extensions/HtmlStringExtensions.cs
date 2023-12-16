using System.Web;

namespace JahanJooy.Common.Util.Web.Extensions
{
	public static class HtmlStringExtensions
	{
		public static IHtmlString If(this IHtmlString input, bool condition)
		{
			return condition ? input : null;
		}
	}
}