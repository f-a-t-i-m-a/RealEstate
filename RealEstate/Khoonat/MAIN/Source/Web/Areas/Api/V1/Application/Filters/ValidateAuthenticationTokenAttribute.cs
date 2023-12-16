using System;
using System.Net;
using System.Web.Mvc;
using Compositional.Composer.Web;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Web.Application.Security;
using log4net;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class ValidateAuthenticationTokenAttribute : ActionFilterAttribute
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateAuthenticationTokenAttribute));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var token = ApiCallContext.Current.InputContext.Token.RemoveWhitespace();

			if (string.IsNullOrWhiteSpace(token))
			{
				// No token, no need to verify.
				// User authentication requirement will be determined and checked in later filters

				ApiCallContext.Current.EndUser = null;
				return;
			}

			var endUser = ParseToken(token);
			if (endUser == null)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.AuthTokenSignatureDoesNotMatch,
			        statusCode: (int) HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext, "Token: " + LogUtils.SanitizeUserInput(token.Truncate(20)));
				return;
			}

			ApiCallContext.Current.EndUser = endUser;
		}

		private CorePrincipal ParseToken(string token)
		{
		    try
		    {
		        var authCookieContents = AuthCookieUtil.Extract(token);
		        if (authCookieContents == null || !authCookieContents.IsValid)
		            return null;

		        var principal = ComposerWebUtil.ComponentContext.GetComponent<IPrincipalCache>()[authCookieContents.UserID];
		        if (principal?.CoreIdentity == null)
                    return null;
                
                if (string.Compare(authCookieContents.LoginName, principal.CoreIdentity.LoginName, StringComparison.OrdinalIgnoreCase) != 0)
		            return null;

		        return principal;
		    }
		    catch (Exception e)
		    {
                Log.Error("Exception while authenticating request", e);
		    }

            return null;
		}
	}
}