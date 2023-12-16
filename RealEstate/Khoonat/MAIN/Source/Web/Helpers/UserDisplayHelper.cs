using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using JahanJooy.RealEstate.Web.Resources.Views.Shared;

namespace JahanJooy.RealEstate.Web.Helpers
{
	public static class UserDisplayHelper
	{
		public static HtmlString UserTag(this HtmlHelper html, string userLoginName, string userDisplayName)
		{
			if (string.IsNullOrWhiteSpace(userLoginName))
				return new HtmlString(LogOnResources.GuestUser);

			return html.ActionLink(userDisplayName, "View", "PublicProfile", new {loginName = userLoginName, Area = ""}, null);
		}
	}
}