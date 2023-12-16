using System;
using System.Net;
using System.Web.Mvc;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Text;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class ValidateApiCallSignatureAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
            // TODO Signature should be moved to HTTP headers.
            // They are removed from input context, so the following lines of code are useless for now.

		    string signatureHash = null; //ApiCallContext.Current.InputContext.SignatureHash.RemoveWhitespace();
		    long signatureTimestamp = 0; //ApiCallContext.Current.InputContext.SignatureTimestamp;

			if (string.IsNullOrWhiteSpace(signatureHash))
			{
				// No signature, no need to verify.
				// Signature requirement will be determined and checked in later filters

				ApiCallContext.Current.MessageSignatureVerified = false;
				return;
			}

			if (signatureTimestamp == 0)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.MessageSignatureTimestampIsMissing,
			        statusCode: (int) HttpStatusCode.Forbidden);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}

			if (signatureTimestamp > DateTime.Now.AddSeconds(120).Ticks ||
			    signatureTimestamp < DateTime.Now.AddSeconds(-120).Ticks)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(
			        ApiErrorCode.MessageSignatureTimestampIsNotInAllowedWindow, statusCode: (int) HttpStatusCode.Forbidden);
			    ApiLogUtils.LogFilteredCall(filterContext,
			        "Input timestamp: " + signatureTimestamp + ", Server timestamp: " + DateTime.Now.Ticks);
				return;
			}

			if (signatureHash.Length != 28)
			{
			    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.MessageSignatureIsNotInCorrectFormat,
			        statusCode: (int) HttpStatusCode.Forbidden);
			    ApiLogUtils.LogFilteredCall(filterContext,
			        "Signature hash length: " + signatureHash.Length + ", contents: " +
			        LogUtils.SanitizeUserInput(signatureHash.Truncate(20)));
				return;
			}

            // TODO: Actually verify signature hash

		    filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.MessageSignatureValidationFailed,
		        statusCode: (int) HttpStatusCode.Forbidden);
		    ApiLogUtils.LogFilteredCall(filterContext,
		        "Timestamp: " + signatureTimestamp + ", Signature: " + LogUtils.SanitizeUserInput(signatureHash.Truncate(20)));
		}
	}
}