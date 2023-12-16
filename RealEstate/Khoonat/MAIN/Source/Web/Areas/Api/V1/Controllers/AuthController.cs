using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.Authentication;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth;
using JahanJooy.RealEstate.Web.Resources.Controllers.Account;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
    public class AuthController : ApiControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IAuthenticationService AuthenticationService { get; set; }

        [HttpPost]
        public ActionResult Login(ApiAuthLoginInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();

            var authResult =
                AuthenticationService.Authenticate(new PasswordAuthenticationRequest
                {
                    LoginName = input.LoginName,
                    Password = input.Password
                });

            var failedResult = authResult as FailedAuthenticationResult;
            if (failedResult != null)
            {
                ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(failedResult.ErrorKey));
                return ValidationErrorFromModelState();
            }

            var successResult = authResult as SuccessfulAuthenticationResult;
            if (successResult?.Principal.CoreIdentity.UserId != null)
            {
                var encryptedTicket = AuthCookieUtil.BuildTicket(
                    new AuthCookieContents
                    {
                        UserID = successResult.Principal.CoreIdentity.UserId.Value,
                        LoginName = successResult.Principal.CoreIdentity.LoginName
                    },
                    true);

                return Json(new ApiAuthLoginOutputModel {Token = encryptedTicket});
            }

            return Error(ApiErrorCode.ValidationError);
        }

        [HttpPost]
        public ActionResult Register(ApiAuthRegisterInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();

            if (ApiCallContext.Current?.Session?.Record == null)
                return Error(ApiErrorCode.SessionIsRequiredForThisApiCall);

            var user = input.ToDomainObject();
            var creationResult = AuthenticationService.CreateUser(user, input.Password);

            if (!creationResult.IsValid)
            {
                foreach (var error in creationResult.Errors)
                    ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(error.FullResourceKey));

                return ValidationErrorFromModelState();
            }

            var userId = creationResult.Result.CoreIdentity.UserId.GetValueOrDefault();

            var result = new ApiAuthRegisterOutputModel();
            result.UserID = userId;
            result.VerifyableContactMethods =
                DbManager.Db.UserContactMethods.Where(
                    cm => cm.UserID == userId && !cm.IsDeleted && cm.IsVerifiable && !cm.IsVerified)
                    .Select(cm => new ApiAuthRegisterOutputContactMethodModel
                    {
                        ID = cm.ID,
                        ContactMethodType = cm.ContactMethodType,
                        ContactMethodText = cm.ContactMethodText
                    }).ToList();

            return Json(result);
        }

        [HttpPost]
        public ActionResult ChangePassword(ApiAuthChangePasswordInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();

            return Error(ApiErrorCode.ApiKeyDailyQuotaReached, 401);

            return Success();
        }
    }
}