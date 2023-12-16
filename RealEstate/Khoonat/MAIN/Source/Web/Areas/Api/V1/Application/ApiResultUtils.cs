using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application
{
	public class ApiResultUtils
	{
		public static ApiOutputErrorModel.ErrorDetails BuildErrorDetails(ApiErrorCode errorCode, string details = null)
		{
			return new ApiOutputErrorModel.ErrorDetails
			       {
				       Code = (int) errorCode,
				       Label = errorCode.ToString(),
				       Message = errorCode.Label(ApiErrorCodeResources.ResourceManager),
				       Details = details ?? string.Empty
			       };
		}

		public static ActionResult BuildErrorResult(ApiOutputErrorModel data, int? statusCode = null)
		{
			return new ServiceStackJsonResult
			{
				Data = data,
				ContentType = null,
				ContentEncoding = null,
                StatusCode = statusCode ?? ServiceStackJsonResult.DefaultErrorStatusCode
			};
		}

		public static ActionResult BuildErrorResult(ApiErrorCode errorCode, string details = null, int? statusCode = null)
		{
			return BuildErrorResult(new ApiOutputErrorModel
			                        {
				                        Error = BuildErrorDetails(errorCode, details)
			                        }, statusCode);
		}
	}
}