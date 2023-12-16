using System;
using System.Web;
using System.Web.Mvc;
using JahanJooy.RealEstate.Core.Security;

namespace JahanJooy.RealEstate.Web.Application.Authorization
{
	public class VerifiedUserOnlyAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}

			var principal = httpContext.User as CorePrincipal;
			return principal != null && principal.CoreIdentity != null && principal.CoreIdentity.IsAuthenticated && principal.IsVerified;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			if (filterContext.Result is HttpUnauthorizedResult)
				if (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated)
					filterContext.Result = new RedirectResult(new UrlHelper(filterContext.RequestContext).
					                                          Action("AboutVerification", "MyProfile", new
						                                                                      {
																								  Area = "",
							                                                                      returnUrl = filterContext.HttpContext.Request.Url,
							                                                                      needsVerification = true
						                                                                      }));
		}
	}
}