using System.Net;
using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class AuthorizeApiInvocationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var apiUser = ApiCallContext.Current.ApiUser;

			if (apiUser.RequireSignature && !ApiCallContext.Current.MessageSignatureVerified)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.MessageSignatureIsRequiredForThisApiKey,
			        statusCode: (int) HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}

			if (apiUser.RequireSessions && ApiCallContext.Current.Session == null)
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.SessionIdIsRequiredForThisApiKey);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}

			if (apiUser.RequireAuthentication && ApiCallContext.Current.EndUser == null)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.AuthTokenIsRequiredForThisApiKey,
			        statusCode: (int) HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}
		}
	}
}