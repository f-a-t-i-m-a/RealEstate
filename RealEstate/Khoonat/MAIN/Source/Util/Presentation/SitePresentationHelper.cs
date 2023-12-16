using System.Web;

namespace JahanJooy.RealEstate.Util.Presentation
{
	public static class SitePresentationHelper
	{
		public static IHtmlString SiteName()
		{
			return new HtmlString("خونه&zwnj;ت");
		}

		public static IHtmlString QuotedSiteName()
		{
			return new HtmlString("«خونه&zwnj;ت»");
		}
	}
}