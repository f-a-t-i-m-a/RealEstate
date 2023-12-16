using System;
using System.Web;
using JahanJooy.RealEstate.Web.Models.Sitemap;

namespace JahanJooy.RealEstate.Web.Helpers.Sitemap
{
	public static class SitemapHelper
	{
		private const string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";

		public static string FormatDateTime(DateTime input)
		{
			return input.ToString(DateTimeFormatString);
		}

		public static IHtmlString LastModTag(DateTime? lastModification)
		{
			if (!lastModification.HasValue)
				return null;
			return new HtmlString("<lastmod>" + FormatDateTime(lastModification.Value) + "</lastmod>");
		}

		public static IHtmlString ChangeFreqTag(SitemapItemChangeFrequency? changeFrequency)
		{
			if (!changeFrequency.HasValue)
				return null;
			return new HtmlString("<changefreq>" + changeFrequency.Value.ToString().ToLower() + "</changefreq>");
		}

		public static IHtmlString PriorityTag(double? priority)
		{
			if (!priority.HasValue)
				return null;
			return new HtmlString("<priority>" + string.Format("{0:0.0}", priority.Value) + "</priority>");
		}
	}
}