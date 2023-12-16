using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public class ApiOutputValidationErrorsModel : ApiOutputErrorModel
	{
		public ApiOutputValidationErrorsModel()
		{
			ValidationErrors = new List<ValidationErrorInfo>();
			Error = ApiResultUtils.BuildErrorDetails(ApiErrorCode.ValidationError);
		}

		public List<ValidationErrorInfo> ValidationErrors { get; set; }

		public class ValidationErrorInfo
		{
			public string PropertyPath { get; set; }
			public string ErrorKey { get; set; }
			public List<string> ErrorParameters { get; set; }
		}

		public static ApiOutputValidationErrorsModel FromModelState(ModelStateDictionary modelState)
		{
			var result = new ApiOutputValidationErrorsModel();

			if (modelState.IsValid)
				return result;

			result.ValidationErrors.AddRange(modelState.Values
				.Where(v => v.Errors != null && v.Errors.Any())
				.SelectMany(v => v.Errors.Select(e => new ValidationErrorInfo() {ErrorKey = e.ErrorMessage})));

			return result;
		}

		public static ApiOutputValidationErrorsModel FromValidationResult(ValidationResult validationResult)
		{
			if (validationResult.IsValid)
				return null;

			return FromValidationErrors(validationResult.Errors);
		}

		public static ApiOutputValidationErrorsModel FromValidationErrors(IEnumerable<ValidationError> validationErrors)
		{
			var result = new ApiOutputValidationErrorsModel();

			if (validationErrors.IsNullOrEmptyEnumerable())
				return result;

			result.ValidationErrors.AddRange(
				validationErrors.Select(e => new ValidationErrorInfo
				                             {
					                             ErrorKey = e.ErrorKey,
					                             PropertyPath = e.PropertyPath,
					                             ErrorParameters = e.ErrorParameters.EmptyIfNull().Select(ep => ep.ToString()).ToList()
				                             }));

			return result;
		}

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<ModelStateDictionary, ApiOutputValidationErrorsModel>().ConvertUsing(FromModelState);
			Mapper.CreateMap<ValidationResult, ApiOutputValidationErrorsModel>().ConvertUsing(FromValidationResult);
			Mapper.CreateMap<IEnumerable<ValidationError>, ApiOutputValidationErrorsModel>().ConvertUsing(FromValidationErrors);
		}
	}
}