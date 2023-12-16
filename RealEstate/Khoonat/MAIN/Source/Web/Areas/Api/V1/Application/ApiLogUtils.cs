using System.Globalization;
using System.Web;
using System.Web.Mvc;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application
{
	public static class ApiLogUtils
	{
		public static void LogFilteredCall(ActionExecutingContext filterContext, string details = null)
		{
			RealEstateStaticLogs.ApiFiltered.Info(FormatLogOutput(filterContext.ActionDescriptor, filterContext.Result, details));
		}

		public static void LogInvokedCall(ActionExecutedContext filterContext)
		{
			RealEstateStaticLogs.ApiInvoked.Info(FormatLogOutput(filterContext.ActionDescriptor, filterContext.Result, null));
		}

		private static string FormatLogOutput(ActionDescriptor actionDescriptor, ActionResult actionResult, string details)
		{
			string actionName = GetActionName(actionDescriptor);
			string resultTitle = GetResultTitle(actionResult);
			string userhostAddress = GetUserHostAddress();

			string result;

			if (!ApiCallContext.Exists)
			{
				result = string.Format("[{0,6}][{1,7}][{2,8}] {3,-40} {4} >>>>> {5,6:F1}ms, {6} ({7}, {8}, {9})",
					"?", "?", "?", actionName.Truncate(40), "   ", 0, resultTitle, userhostAddress, "?", "?");
			}
			else
			{
				string apiUserId = ApiCallContext.Current.ApiUser.IfNotNull(u => u.ID.ToString(CultureInfo.InvariantCulture), "?");
				string endUserId = ApiCallContext.Current.EndUser.IfNotNull(u => u.CoreIdentity?.UserId.ToString(), "?");
				string sessionId = ApiCallContext.Current.Session?.Record.IfNotNull(r => r.ID.ToString(CultureInfo.InvariantCulture), "?");


				string signature = ApiCallContext.Current.MessageSignatureVerified ? "SGN" : "   ";
				string userAgent = ApiCallContext.Current.UserAgent;
				string deviceId = ApiCallContext.Current.DeviceId;
				double elapsedTime = ApiCallContext.Current.Stopwatch.IfNotNull(w => w.Elapsed.TotalMilliseconds, -1);


				// Sample output line:
				// [     0][      ?][       ?] Controller.Action                        SGN >>>>>   74.1ms, OK (UserAgent, DeviceId)

				result = string.Format("[{0,6}][{1,7}][{2,8}] {3,-40} {4} >>>>> {5,6:F1}ms, {6} ({7}, {8}, {9})",
					apiUserId, endUserId, sessionId, actionName.Truncate(40), signature, elapsedTime, resultTitle, userhostAddress, userAgent.Truncate(50), deviceId.Truncate(50));

			}

			if (!string.IsNullOrWhiteSpace(details))
				result = result + "; " + details;

			return result;
		}

		private static string GetActionName(ActionDescriptor actionDescriptor)
		{
			if (actionDescriptor == null)
				return "NoActionDesc";

			string controllerName = actionDescriptor.ControllerDescriptor.IfNotNull(cd => cd.ControllerName, "NoCtrlDesc");
			string actionName = actionDescriptor.ActionName;

			if (string.IsNullOrWhiteSpace(controllerName))
				controllerName = "NULL";

			if (string.IsNullOrWhiteSpace(actionName))
				actionName = "NULL";

			return controllerName + "." + actionName;
		}

		private static string GetResultTitle(ActionResult result)
		{
			var jsonResult = result as JsonResult;
			var serviceStackJsonResult = result as ServiceStackJsonResult;

			if (jsonResult == null && serviceStackJsonResult == null)
				return "UnknownResultType";

			var outputModel = serviceStackJsonResult.IfNotNull(r => r.Data) as ApiOutputModelBase ??
			                  jsonResult.IfNotNull(r => r.Data) as ApiOutputModelBase;

			if (outputModel == null)
				return "UnknownOutputDataType";

			var errorModel = outputModel as ApiOutputErrorModel;
			if (errorModel == null)
				return "OK";

			if (errorModel.Error == null)
				return "Error(Null)";

			return errorModel.Error.Label + "(" + errorModel.Error.Code + ")";
		}

		private static string GetUserHostAddress()
		{
			var result = HttpContext.Current.IfNotNull(c => c.Request.IfNotNull(r => r.UserHostAddress));
			return string.IsNullOrWhiteSpace(result) ? "0.0.0.0" : result;
		}
	}
}