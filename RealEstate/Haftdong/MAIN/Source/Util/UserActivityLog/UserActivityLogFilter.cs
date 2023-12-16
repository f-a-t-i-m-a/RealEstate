using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    public class UserActivityLogFilter : ActionFilterAttribute
    {
        #region Overrides of ActionFilterAttribute

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            SetContextValues(actionContext);
            base.OnActionExecuting(actionContext);
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            SetContextValues(actionContext);
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        #endregion

        private static void SetContextValues(HttpActionContext actionContext)
        {
            var entry = UserActivityLogUtils.CurrentEntry;
            var actionDescriptor = actionContext.IfNotNull(ac => ac.ActionDescriptor);

            entry.ActionName = actionDescriptor.IfNotNull(ad => ad.ActionName);
            entry.ControllerName = actionDescriptor.IfNotNull(ad => ad.ControllerDescriptor.IfNotNull(
                cd => cd.ControllerName));

            var skipAttributes = actionContext.ActionDescriptor.GetCustomAttributes<SkipUserActivityLoggingAttribute>();
            if (skipAttributes.SafeAny())
            {
                entry.ClearMainActivity();
            }
            else
            {
                var userActivityAttributes = actionContext.ActionDescriptor.GetCustomAttributes<UserActivityAttribute>();
                if (userActivityAttributes.SafeAny())
                {
                    entry.EnsureMainActivity();
                    userActivityAttributes.First().Apply(entry.MainActivity);
                }
                else
                {
                    if (entry.MainActivity != null)
                        throw new InvalidOperationException("[UserActivity] attribute should be specified for this action.");
                }
            }
        }

    }
}