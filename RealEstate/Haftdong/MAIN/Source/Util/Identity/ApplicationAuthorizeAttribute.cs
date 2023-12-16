using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using JahanJooy.RealEstateAgency.Domain.Enums.User;

namespace JahanJooy.RealEstateAgency.Util.Identity
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        public BuiltInRole[] BuiltInRoles { get; set; }

        #region Overrides of AuthorizeAttribute

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            // Administrator role has all the permissions
            if (user.IsInRole(BuiltInRole.Administrator.ToString()))
                return true;

            if (BuiltInRoles != null && BuiltInRoles.Length > 0 && BuiltInRoles.All(r => !user.IsInRole(r.ToString())))
                return false;

            return base.IsAuthorized(actionContext);
        }

        #endregion
    }
}