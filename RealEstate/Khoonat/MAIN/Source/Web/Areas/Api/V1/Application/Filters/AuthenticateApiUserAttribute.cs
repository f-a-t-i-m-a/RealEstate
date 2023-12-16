using System;
using System.Net;
using System.Web.Mvc;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class AuthenticateApiUserAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var apiKey = ApiCallContext.Current.InputContext.ApiKey.RemoveWhitespace();

			if (string.IsNullOrWhiteSpace(apiKey))
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.ApiKeyIsMissing, statusCode: (int)HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}

			if (apiKey.Length != 32)
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.ApiKeyIsNotInCorrectFormat, statusCode: (int)HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext, "Input API Key length is " + apiKey.Length);
				return;
			}

			var apiUser = TemporaryApiUserStore.LookupApiKey(apiKey);
			if (apiUser == null)
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.ApiKeyIsInvalid, statusCode: (int)HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext, "Input API Key is " + LogUtils.SanitizeUserInput(apiKey.Truncate(20)));
				return;
			}

			if (apiUser.ExpirationTime.HasValue && apiUser.ExpirationTime.Value < DateTime.Now)
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.ApiKeyIsExpired, statusCode: (int)HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext, "API Key " + LogUtils.SanitizeUserInput(apiKey.Truncate(20)) + " has expired on " + apiUser.ExpirationTime.Value);
				return;
			}

			ApiCallContext.Current.ApiUser = apiUser;
		}

		private static class TemporaryApiUserStore
		{
			private const string AndroidAppApiKey = "aSR/bM+6sK54wxkxFIE8eujLxTxWMQc/";

			public static ApiUser LookupApiKey(string key)
			{
				if (key == AndroidAppApiKey)
					return GetAndriodAppApiUser();

				return null;
			}

			private static ApiUser GetAndriodAppApiUser()
			{
				return new ApiUser
				{
					ID = 0,
					Key = AndroidAppApiKey,
					CreationTime = DateTime.Now,
					LastModificationTime = DateTime.Now,
					ExpirationTime = null,
					RequestedByUserID = 1,
					ReviewedByUserID = 1,
					DailyCallQuota = -1,
					DailyCallQuotaPerUser = -1,
					RequireSessions = false,
					RequireSignature = false,
					RequireAuthentication = false
				};
			}
		}
	}
}