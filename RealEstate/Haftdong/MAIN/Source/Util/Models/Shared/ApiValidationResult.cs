using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using JahanJooy.Common.Util.Validation;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Shared
{
	[TsClass]
    public class ApiValidationResult
	{
		public bool Success { get; set; }
		public List<ApiValidationError> Errors { get; set; }

		public static ApiValidationResult Ok()
		{
			return new ApiValidationResult {Success = true, Errors = null};
		}

		public static ApiValidationResult Failure(ApiValidationError error)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {error}};
		}

		public static ApiValidationResult Failure(IEnumerable<ApiValidationError> errors)
		{
			return new ApiValidationResult {Success = false, Errors = errors.ToList()};
		}

		
		public static ApiValidationResult Failure(ValidationError error)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {Mapper.Map<ApiValidationError>(error)}};
		}

		public static ApiValidationResult Failure(IEnumerable<ValidationError> errors)
		{
			return new ApiValidationResult {Success = false, Errors = errors.Select(Mapper.Map<ApiValidationError>).ToList()};
		}
		
		public static ApiValidationResult Failure(string errorKey)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {new ApiValidationError(errorKey)}};
		}

		public static ApiValidationResult Failure(string errorKey, IEnumerable<string> errorParameters)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {new ApiValidationError(errorKey, errorParameters)}};
		}

		public static ApiValidationResult Failure(string propertyPath, string errorKey)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {new ApiValidationError(propertyPath, errorKey)}};
		}

		public static ApiValidationResult Failure(string propertyPath, string errorKey, IEnumerable<string> errorParameters)
		{
			return new ApiValidationResult {Success = false, Errors = new List<ApiValidationError> {new ApiValidationError(propertyPath, errorKey, errorParameters)}};
		}

		public static ApiValidationResult FromValidationResult(ValidationResult validationResult)
		{
			if (validationResult.IsValid)
				return Ok();

			return new ApiValidationResult {Success = false, Errors = Mapper.Map<List<ApiValidationError>>(validationResult.Errors)};
		}
	}
}