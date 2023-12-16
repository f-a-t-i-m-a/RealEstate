using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.Common.Util.Web.Captcha;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.Authentication;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Application.Session;
using JahanJooy.RealEstate.Web.Models.Account;
using JahanJooy.RealEstate.Web.Resources.Controllers.Account;
using JahanJooy.Common.Util.Web.Session;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class AccountController : CustomControllerBase
	{
		[ComponentPlug]
		public IAuthenticationService AuthenticationService { get; set; }

        [ComponentPlug]
        public IPrincipalCache PrincipalCache { get; set; }

		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

		[HttpGet]
		[RequireHttps]
		public ActionResult LogOn(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
				return View("LogOnAlreadyAuthenticated");

			var model = new AccountLogOnModel
			            {
				            AcquireOwnedProperties = true
			            };

			return View(model);
		}

		[HttpPost]
		[RequireHttps]
		[ActionName("LogOn")]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult LogOnPostback(AccountLogOnModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
				return View(model);

			var authResult = AuthenticationService.Authenticate(new PasswordAuthenticationRequest {LoginName = model.LoginName, Password = model.Password});
			
			var failedResult = authResult as FailedAuthenticationResult;
			if (failedResult != null)
			{
				ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(failedResult.ErrorKey));
				return View(model);
			}

			var successResult = authResult as SuccessfulAuthenticationResult;
			if (successResult != null && successResult.Principal.CoreIdentity.UserId.HasValue)
			{
				AuthenticationService.EndSession(Session.GetSessionInfo(), SessionEndReason.UserAuthenticated);
				AuthCookieUtil.Set(new AuthCookieContents {UserID = successResult.Principal.CoreIdentity.UserId.Value, LoginName = successResult.Principal.CoreIdentity.LoginName},
				                   model.RememberMe);

				// Acquire owned properties, if the user is not Admin
				if (model.AcquireOwnedProperties && SessionInfo != null && SessionInfo.OwnedProperties != null && SessionInfo.OwnedProperties.Count > 0 && !successResult.Principal.IsOperator)
				{
					PropertyService.SetOwner(SessionInfo.OwnedProperties, successResult.Principal.CoreIdentity.UserId.Value);
					SessionInfo.OwnedProperties = null;
					OwnedPropertiesCookieUtil.Set(null);
				}

				Session.AbandonAndRenewCookie(Response);

                if (!successResult.Principal.IsVerified && !(returnUrl ?? "").Contains("VerifyEmailDirectLink"))
				{
					return RedirectToAction("CompleteRegistration", "MyProfile", new {returnUrl});
				}

				if (string.IsNullOrWhiteSpace(returnUrl) && successResult.Principal.IsOperator)
					return RedirectToAction("Index", "AdminHome", new {Area = "Admin"});

				return Redirect(GetReturnUrl(returnUrl));
			}

			return View(model);
		}

		[HttpGet]
		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			AuthenticationService.EndSession(Session.GetSessionInfo(), SessionEndReason.Logout);
			Session.AbandonAndRenewCookie(Response);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[RequireHttps]
		public ActionResult Register()
		{
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));

			var model = new AccountSignUpModel
				            {
					            DisplayNameSameAsFullName = true,
					            EmailVisibility = VisibilityLevel.Everybody,
                                Phone1Visibility = VisibilityLevel.Everybody,
                                Phone2Visibility = VisibilityLevel.Everybody,
								AcquireOwnedProperties = true,
                                IndependentAgent = true
				            };
		    return View(model);
		}

		[HttpPost]
		[RequireHttps]
		[ActionName("Register")]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult RegisterPostback(AccountSignUpModel model)
		{
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));

            if (!ModelState.IsValid)
                return View(model);

		    var user = model.ToDomainObject();
			var creationResult = AuthenticationService.CreateUser(user, model.Password);

		    if (!creationResult.IsValid)
		    {
                foreach (var error in creationResult.Errors)
                    ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(error.FullResourceKey));

                return View(model);
            }

			var userId = creationResult.Result.CoreIdentity.UserId.GetValueOrDefault();
			var verifiableContactMethodIds = DbManager.Db.UserContactMethods.Where(cm => cm.UserID == userId && !cm.IsDeleted && cm.IsVerifiable && !cm.IsVerified)
				.Select(cm => cm.ID).ToList();

			foreach (var verifiableContactMethodId in verifiableContactMethodIds)
			{
				AuthenticationService.StartContactMethodVerification(verifiableContactMethodId);
			}

			AuthenticationService.EndSession(Session.GetSessionInfo(), SessionEndReason.UserRegistered);
			AuthCookieUtil.Set(new AuthCookieContents { UserID = userId, LoginName = creationResult.Result.CoreIdentity.LoginName }, false);

			// Acquire owned properties, if any
			if (model.AcquireOwnedProperties && SessionInfo != null && SessionInfo.OwnedProperties != null && SessionInfo.OwnedProperties.Count > 0 && creationResult.Result.CoreIdentity.UserId.HasValue)
			{
				PropertyService.SetOwner(SessionInfo.OwnedProperties, creationResult.Result.CoreIdentity.UserId.Value);
				SessionInfo.OwnedProperties = null;
				OwnedPropertiesCookieUtil.Set(null);
			}

			Session.AbandonAndRenewCookie(Response);
			return RedirectToAction("CompleteRegistration", "MyProfile");
        }

        [HttpGet]
		[RequireHttps]
		public ActionResult CheckLoginName(string loginName)
		{
			// TODO: Prevent direct calls (allow calls from Register page only)
			bool result = !(AuthenticationService.CheckLoginNameExists(loginName));
			return Json(result, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
		[RequireHttps]
		public ActionResult ForgotPassword()
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));

			var model = new AccountForgotPasswordModel();
            return View(model);
        }

        [HttpPost]
		[RequireHttps]
		[ActionName("ForgotPassword")]
		[SubmitButton("btnStartPasswordRecoveryBySms")]
		[ValidateCaptcha(validateImageCaptcha: true)]
		public ActionResult ForgotPasswordPostbackBySms(AccountForgotPasswordModel model)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));

			if (string.IsNullOrWhiteSpace(model.VerificationPhoneNumber))
				ModelState.AddModelError("VerificationPhoneNumber", AccountValidationErrors.ResourceManager.GetString("Validation_VerificationPhoneNumber_Required"));

			if (!ModelState.IsValid)
                return ForgotPassword();

            var startRecoveryResult = AuthenticationService.StartPasswordRecovery(model.LoginName, ContactMethodType.Phone, model.VerificationPhoneNumber);

            if (startRecoveryResult.IsValid)
                return RedirectToAction("ResetPassword", new {loginName = model.LoginName});

            foreach (var error in startRecoveryResult.Errors)
				ModelState.AddModelError("VerificationPhoneNumber", AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));

            return View();
        }

        [HttpPost]
		[RequireHttps]
		[ActionName("ForgotPassword")]
		[SubmitButton("btnStartPasswordRecoveryByEmail")]
		[ValidateCaptcha(validateImageCaptcha: true)]
		public ActionResult ForgotPasswordPostbackByEmail(AccountForgotPasswordModel model)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));

			if (string.IsNullOrWhiteSpace(model.VerificationEmailAddress))
				ModelState.AddModelError("VerificationEmailAddress", AccountValidationErrors.ResourceManager.GetString("Validation_VerificationEmailAddress_Required"));

			if (!ModelState.IsValid)
                return ForgotPassword();

            var startRecoveryResult = AuthenticationService.StartPasswordRecovery(model.LoginName, ContactMethodType.Email, model.VerificationEmailAddress);

            if (startRecoveryResult.IsValid)
                return RedirectToAction("ResetPassword", new {loginName = model.LoginName});

            foreach (var error in startRecoveryResult.Errors)
				ModelState.AddModelError("VerificationEmailAddress", AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));

            return View();
        }

        [HttpGet]
		[RequireHttps]
		public ActionResult ResetPassword(string loginName, string passwordResetToken)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));
			
			var model = new AccountResetPasswordModel
                            {
                                LoginName = loginName ?? "",
								PasswordResetToken = passwordResetToken ?? ""
                            };

            return View(model);
        }

        [HttpPost]
		[RequireHttps]
		[ActionName("ResetPassword")]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult ResetPasswordPostback(AccountResetPasswordModel model)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));
			
			if (!ModelState.IsValid)
                return View(model);

            var performRecoveryResult = AuthenticationService.PerformPasswordRecovery(model.LoginName, model.PasswordResetToken, model.NewPassword);
            if (performRecoveryResult.IsValid)
                return RedirectToAction("ResetPasswordComplete");

            foreach (var error in performRecoveryResult.Errors)
                ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));

            return View(model);
        }

        [HttpGet]
		[RequireHttps]
		public ActionResult ResetPasswordComplete()
        {
            return View();
        }

        [HttpGet]
		public ActionResult ForgotLoginName()
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));
			
			var model = new AccountForgotLoginNameModel();
            return View(model);
        }

        [HttpPost]
		[ActionName("ForgotLoginName")]
        [SubmitButton("btnQueryUsernameBySms")]
		[ValidateCaptcha(validateImageCaptcha: true)]
		public ActionResult ForgotUsernamePostbackBySms(AccountForgotLoginNameModel model)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));
			
			if (string.IsNullOrWhiteSpace(model.VerificationPhoneNumber))
                ModelState.AddModelError("VerificationPhoneNumber", AccountValidationErrors.ResourceManager.GetString("Validation_VerificationPhoneNumber_Required"));

            if (!ModelState.IsValid)
                return View(model);

            var performRecoveryResult = AuthenticationService.PerformLoginNameRecovery(ContactMethodType.Phone, model.VerificationPhoneNumber);
            if (performRecoveryResult.IsValid)
                return RedirectToAction("LoginNameQueryComplete");

            foreach (var error in performRecoveryResult.Errors)
                ModelState.AddModelError("VerificationPhoneNumber", AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));

            return View(model);
        }

        [HttpPost]
        [ActionName("ForgotLoginName")]
        [SubmitButton("btnQueryUsernameByEmail")]
		[ValidateCaptcha(validateImageCaptcha: true)]
        public ActionResult ForgotUsernamePostbackByEmail(AccountForgotLoginNameModel model)
        {
			if (User.Identity.IsAuthenticated)
				return Redirect(GetReturnUrl(null));
			
			if (string.IsNullOrWhiteSpace(model.VerificationEmailAddress))
                ModelState.AddModelError("VerificationEmailAddress", AccountValidationErrors.ResourceManager.GetString("Validation_VerificationEmailAddress_Required"));

            if (!ModelState.IsValid)
                return View(model);

            var performRecoveryResult = AuthenticationService.PerformLoginNameRecovery(ContactMethodType.Email, model.VerificationEmailAddress);
            if (performRecoveryResult.IsValid)
                return RedirectToAction("LoginNameQueryComplete");

            foreach (var error in performRecoveryResult.Errors)
                ModelState.AddModelError("VerificationEmailAddress", AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));

            return View(model);
        }

        [HttpGet]
        public ActionResult LoginNameQueryComplete()
        {
            return View();
        }

		[HttpGet]
		[RequireHttps]
		[Authorize]
		public ActionResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		[RequireHttps]
		[Authorize]
		[ActionName("ChangePassword")]
		public ActionResult ChangePasswordPostback(AccountChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var changeResult = AuthenticationService.ChangePassword(model.OldPassword, model.NewPassword);
				if (changeResult.IsValid)
					return RedirectToAction("ChangePasswordSuccess");

				foreach (var error in changeResult.Errors)
					ModelState.AddModelError("", AccountValidationErrors.ResourceManager.GetString(error.FullResourceKey));
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[Authorize]
		[RequireHttps]
		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}

        #region Private helper methods

        private string GetReturnUrl(string originalReturnUrl)
        {
            if (!string.IsNullOrWhiteSpace(originalReturnUrl) &&
                Url.IsLocalUrl(originalReturnUrl) &&
                originalReturnUrl.Length > 1 &&
                originalReturnUrl.StartsWith("/") &&
                !originalReturnUrl.StartsWith("//") &&
                !originalReturnUrl.StartsWith("/\\") && 
				!originalReturnUrl.Contains("\r")&& 
				!originalReturnUrl.Contains("\n"))
            {
                return originalReturnUrl;
            }

            return Url.Action("Index", "Home");
        }

		#endregion

	}
}
