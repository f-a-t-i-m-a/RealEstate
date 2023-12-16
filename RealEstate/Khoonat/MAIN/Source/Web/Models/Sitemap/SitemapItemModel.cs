using System;

namespace JahanJooy.RealEstate.Web.Models.Sitemap
{
	public class SitemapItemModel
	{
		public string Url { get; set; }
		public DateTime? LastModification { get; set; }
		public SitemapItemChangeFrequency? ChangeFrequency { get; set; }
		public double? Priority { get; set; }
	}

	public enum SitemapItemChangeFrequency
	{
		Always,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
		Never
	}
}