using System.Web.Mvc;
using Compositional.Composer.Web;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Web.Application.Security;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class ValidateSessionIdAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var sessionIdString = ApiCallContext.Current.InputContext.SessionId.RemoveWhitespace();

			if (string.IsNullOrWhiteSpace(sessionIdString))
			{
				// No session ID, no need to verify.
				// Session requirement will be determined and checked in later filters

				ApiCallContext.Current.Session = null;
				return;
			}

		    var httpSession = ComposerWebUtil.ComponentContext.GetComponent<IHttpSessionCache>()[sessionIdString];
		    if (httpSession == null)
		    {
                filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.SessionIdIsInvalid);
                ApiLogUtils.LogFilteredCall(filterContext, "Input Session ID: " + LogUtils.SanitizeUserInput(sessionIdString.Truncate(10)));
		        return;
		    }

		    if (httpSession.EndReason.HasValue)
		    {
                filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.SessionIsExpired);
                ApiLogUtils.LogFilteredCall(filterContext, "Input Session ID: " + LogUtils.SanitizeUserInput(sessionIdString.Truncate(10)));
                return;
            }

		    if (httpSession.UserID.HasValue && ApiCallContext.Current.EndUser == null)
		    {
                filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.SessionIsAuthenticatedAndNeedsCredentials);
                ApiLogUtils.LogFilteredCall(filterContext, "Input Session ID: " + LogUtils.SanitizeUserInput(sessionIdString.Truncate(10)));
                return;
            }

		    ApiCallContext.Current.Session = new SessionInfo {Record = httpSession};
		}
	}
}