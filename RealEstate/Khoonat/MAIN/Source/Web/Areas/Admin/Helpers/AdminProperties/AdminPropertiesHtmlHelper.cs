using System.Web.Mvc;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Helpers.AdminProperties
{
	public static class AdminPropertiesHtmlHelper
	{
		public static string PropertyListingStyle(this HtmlHelper html, PropertyListingSummary prop)
		{
			return html.PropertyListingStyle(prop.IsPublished(), prop.Approved, prop.DeleteDate.HasValue);
		}

		public static string PropertyListingStyle(this HtmlHelper html, bool isPublished, bool? approved, bool deleted)
		{
			string bkgColor;

			if (isPublished)
			{
				if (!approved.HasValue)
					bkgColor = "#ffaaaa";
				else
					bkgColor = approved.Value ? "#ddffdd" : "#ffffff";
			}
			else if (!approved.HasValue)
			{
				bkgColor = "#ffcccc";
			}
			else
			{
				bkgColor = approved.Value ? "#bbddbb" : "#dddddd";
				
			}

			return (deleted ? "text-decoration: line-through; " : "") + "background-color: " + bkgColor + "; ";
		}
	}
}