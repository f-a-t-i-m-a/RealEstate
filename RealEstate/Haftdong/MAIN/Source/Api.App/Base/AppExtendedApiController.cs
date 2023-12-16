using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Results;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Util.Models.Shared;

namespace JahanJooy.RealEstateAgency.Api.App.Base
{
    [Authorize]
    public abstract class AppExtendedApiController : ApiController
    {
        protected IHttpActionResult ValidationResult(ValidationResult result)
        {
            if (result.IsValid)
                return Ok();

            return new ValidationErrorResult(ApiValidationResult.FromValidationResult(result), this);
        }

        protected IHttpActionResult ValidationResult(string propertyPath, string errorKey)
        {
            return new ValidationErrorResult(ApiValidationResult.Failure(propertyPath, errorKey), this);
        }

        protected IHttpActionResult ValidationError(ValidationError error)
        {
            return new ValidationErrorResult(ApiValidationResult.Failure(error), this);
        }
        protected IHttpActionResult ValidationError(ApiValidationError error)
        {
            return new ValidationErrorResult(ApiValidationResult.Failure(error), this);
        }
    }

    public class ValidationErrorResult : NegotiatedContentResult<ApiValidationResult>
    {
        public ValidationErrorResult(ApiValidationResult content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            : base(HttpStatusCode.BadRequest, content, contentNegotiator, request, formatters)
        {
        }

        public ValidationErrorResult(ApiValidationResult content, ApiController controller)
            : base(HttpStatusCode.BadRequest, content, controller)
        {
        }
    }
}