using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredPropertyAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Helper
{
    public static class SponsoredPropertyAdminHtmlHelper
    {
        public static string SponsoredPropertyStyle(this HtmlHelper html, SponsoredPropertyAdminListItemModel sponsoredProperty)
        {
            return html.SponsoredPropertyStyle(sponsoredProperty.Approved);
        }

        public static string SponsoredPropertyStyle(this HtmlHelper html, bool? approved)
        {
            string bkgColor;

           
                if (!approved.HasValue)
                    bkgColor = "#ffaaaa";
                else
                    bkgColor = approved.Value ? "#ddffdd" : "#ffffff";

            return "background-color: " + bkgColor + "; ";
        }
    }
}