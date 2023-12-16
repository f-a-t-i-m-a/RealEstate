using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer.Web;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class SynchronizeApiCallContextAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (ApiCallContext.Exists)
				throw new InvalidOperationException("An ApiCallContext already exists when entering the API filter chain.");

			// Create ApiCallContext

			var input = filterContext.ActionParameters.Select(p => p.Value).OfType<ApiInputModelBase>().FirstOrDefault();
			if (input?.Context == null)
			{
				filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.InputContextIsMissing);
				ApiLogUtils.LogFilteredCall(filterContext);
				return;
			}

			ApiCallContext.Create(input.Context);

			// Set ClientCulture, if provided

			if (!string.IsNullOrWhiteSpace(ApiCallContext.Current.InputContext.UserCulture))
			{
				try
				{
					var userCulture = CultureInfo.GetCultureInfo(ApiCallContext.Current.InputContext.UserCulture);
					if (!IsCultureSupported(userCulture))
					{
						filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.UserCultureIsNotSupported);
						ApiLogUtils.LogFilteredCall(filterContext, "Culture string: " + LogUtils.SanitizeUserInput(ApiCallContext.Current.InputContext.UserCulture));
						return;
					}

					ApiCallContext.Current.UserCulture = userCulture;
				}
				catch (CultureNotFoundException)
				{
					filterContext.Result = ApiResultUtils.BuildErrorResult(ApiErrorCode.UserCultureIsNotIdentified);
					ApiLogUtils.LogFilteredCall(filterContext, "Culture string: " + LogUtils.SanitizeUserInput(ApiCallContext.Current.InputContext.UserCulture));
					return;
				}
			}
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var outputModel = (filterContext.Result as ServiceStackJsonResult).IfNotNull(r => r.Data as ApiOutputModelBase) ??
			                  (filterContext.Result as JsonResult).IfNotNull(r => r.Data as ApiOutputModelBase);

			if (outputModel != null)
			{
				if (outputModel.Context == null)
					outputModel.Context = new ApiOutputContextModel();

				outputModel.Context.AppVersion = ComposerWebUtil.ComponentContext.GetComponent<ApplicationRevisionInfo>().VersionString;
				outputModel.Context.ApiVersion = ComposerWebUtil.ComponentContext.GetComponent<ApplicationRevisionInfo>().ApiVersionString;
				outputModel.Context.IsObsolete = ApiCallContext.Current.MarkedAsObsolete;
				outputModel.Context.ServerMessages = ApiCallContext.Current.ServerMessages;
				outputModel.Context.CustomMessages = ApiCallContext.Current.CustomMessages;
			}
		}

		private bool IsCultureSupported(CultureInfo culture)
		{
			return culture.Equals(CultureInfo.InvariantCulture);
		}
	}
}