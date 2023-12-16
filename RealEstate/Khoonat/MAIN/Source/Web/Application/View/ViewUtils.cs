using System.Web;

namespace JahanJooy.RealEstate.Web.Application.View
{
	public static class ViewUtils
	{
		public static string GetBodyActionId()
		{
			return (HttpContext.Current.Request.RequestContext.RouteData.Values["area"] ?? "main").ToString().ToLower() +
			       HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower() +
			       HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
		}
	}
}