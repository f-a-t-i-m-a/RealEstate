using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Validation;
using JahanJooy.Common.Util.Web.Attributes.Filters;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application
{
	[RejectNonSecureInProduction]
	[SessionState(SessionStateBehavior.Disabled)]
	[SynchronizeApiCallContext(Order = 1)]
	[AuthenticateApiUser(Order = 2)]
	[ValidateApiCallSignature(Order = 3)]
	[ValidateAuthenticationToken(Order = 4)]
	[ValidateSessionId(Order = 5)]
	[AuthorizeApiInvocation(Order = 6)]
	[LogApiInvocation(Order = 10)]
    [ApiServiceContextActionFilter(Order = 11)]
	public class ApiControllerBase : CustomControllerBase
	{
		protected override ActionResult Error(ErrorResult error)
		{
			switch (error)
			{
				case ErrorResult.AccessDenied:
					return Error(ApiErrorCode.MethodInvocationIsForbidden);

				case ErrorResult.EntityNotFound:
					return Error(ApiErrorCode.EntityNotFound);
			}

			return Error(ApiErrorCode.UnknownError);
		}

		protected ActionResult Error(ApiErrorCode errorCode, int? statusCode = null)
		{
			return ApiResultUtils.BuildErrorResult(errorCode, statusCode: statusCode);
		}

		protected ActionResult InputIsEmptyError(string propertyPath)
		{
			return ApiResultUtils.BuildErrorResult(ApiErrorCode.InputIsEmpty, "Property path: " + propertyPath);
		}

		protected ActionResult ValidationError(List<ValidationError> errors)
		{
			if (errors.IsNullOrEmptyEnumerable())
				return Error(ApiErrorCode.ValidationError);

			return ApiResultUtils.BuildErrorResult(ApiOutputValidationErrorsModel.FromValidationErrors(errors));
		}

		protected ActionResult ValidationError(ValidationResult validationResult)
		{
			if (validationResult.IsValid)
				return Error(ApiErrorCode.ValidationError);

			return ApiResultUtils.BuildErrorResult(ApiOutputValidationErrorsModel.FromValidationResult(validationResult));
		}

		protected ActionResult ValidationErrorFromModelState(ModelStateDictionary modelState)
		{
			return ApiResultUtils.BuildErrorResult(ApiOutputValidationErrorsModel.FromModelState(modelState));
		}

		protected ActionResult ValidationErrorFromModelState()
		{
			return ValidationErrorFromModelState(ModelState);
		}

		protected ActionResult Success()
		{
			return Json(new ApiOutputSuccessModel {Success = true});
		}
	}
}