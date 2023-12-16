using System;
using System.Web;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Domain.Permissions;

namespace JahanJooy.RealEstate.Web.Application.Authorization
{
    public class AuthorizeGeneralPermissionAttribute : AuthorizeAttribute
    {
        private readonly GeneralPermission _permission;

        public AuthorizeGeneralPermissionAttribute(GeneralPermission permission)
        {
            _permission = permission;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var principal = httpContext.User as CorePrincipal;
            return principal != null && principal.CoreIdentity.IsAuthenticated && principal.HasPermission(_permission);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
                if (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated)
                    filterContext.Result = new TransferResult(new UrlHelper(filterContext.RequestContext).Action("AccessDenied", "Error", new { Area = "" }));
        }
    }
}