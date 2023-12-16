using System;
using System.Web;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Routing;

namespace JahanJooy.RealEstate.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string SetSortOrder(this UrlHelper url, object order, object currentOrder = null, bool? currentDescending = null, string sortOrderParamName = "SortOrder", string sortDescendingParamName = "SortDescending")
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.Equals(currentOrder))
                return url.RouteUrl(HttpContext.Current.Request.QueryString.ToRouteValues().Set(sortDescendingParamName, (!(currentDescending.GetValueOrDefault(false))).ToString()));

            return url.RouteUrl(HttpContext.Current.Request.QueryString.ToRouteValues().Set(sortOrderParamName, order.ToString()));
        }
    }
}