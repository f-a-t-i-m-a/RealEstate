using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using JahanJooy.Common.Util.Web.Captcha.Image;

namespace JahanJooy.Common.Util.Web.Captcha
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ValidateCaptchaAttribute : FilterAttribute, IAuthorizationFilter
	{
		private readonly bool _validateScriptCaptcha;
		private readonly bool _validateImageCaptcha;

		public ValidateCaptchaAttribute(
			bool validateScriptCaptcha = false, 
			bool validateImageCaptcha = false)
		{
			if (!validateImageCaptcha && !validateScriptCaptcha)
				throw new ArgumentException("None of the validations specified in the attribute constructor");

			_validateScriptCaptcha = validateScriptCaptcha;
			_validateImageCaptcha = validateImageCaptcha;
		}

		/// <summary>
		/// Authorizes the current action by validating spam prevention responses.
		/// </summary>
		/// <param name="filterContext">The filter's authorization context.</param>
		public void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException(nameof(filterContext));
			if (filterContext.HttpContext == null)
				throw new ArgumentException("HttpContext in filterContext must not be null");
			if (filterContext.Controller?.ViewData?.ModelState == null)
				throw new ArgumentException("ModelState in filterContext must not be null");

			if (_validateScriptCaptcha)
				AuthorizeScriptCaptcha(filterContext.HttpContext, filterContext.Controller.ViewData.ModelState);

			if (_validateImageCaptcha)
				AuthorizeImageCaptcha(filterContext.HttpContext, filterContext.Controller.ViewData.ModelState);
		}

		/// <summary>
		/// Authorizes script-based spam prevention responses. If validation fails, updates model state accordingly.
		/// </summary>
		/// <param name="httpContext">The request's HTTP context.</param>
		/// <param name="modelState">The request's model state.</param>
		public static void AuthorizeScriptCaptcha(HttpContextBase httpContext, ModelStateDictionary modelState)
		{
			if (httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));
			if (modelState == null)
				throw new ArgumentNullException(nameof(modelState));

			var scriptCaptchaResponseIsValid = ValidateScriptCaptchaResponse(httpContext);
			if (!scriptCaptchaResponseIsValid)
			{
				modelState.AddModelError(CaptchaConstants.ModelStateKeyForScriptCaptcha, string.Empty);
			}
		}

		/// <summary>
		/// Authorizes image-based spam prevention responses. If validation fails, updates model state accordingly.
		/// </summary>
		/// <param name="httpContext">The request's HTTP context.</param>
		/// <param name="modelState">The request's model state.</param>
		public static void AuthorizeImageCaptcha(HttpContextBase httpContext, ModelStateDictionary modelState)
		{
			if (httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));
			if (modelState == null)
				throw new ArgumentNullException(nameof(modelState));

			var imageCaptchaResponseIsValid = ValidateImageCaptchaResponse(httpContext);
			if (!imageCaptchaResponseIsValid)
			{
				modelState.AddModelError(CaptchaConstants.ModelStateKeyForImageCaptcha, string.Empty);
			}
		}

		public static bool ValidateScriptCaptchaResponse(HttpContextBase httpContext)
		{
			var scriptCaptchaChallenge = httpContext.Request.Form[CaptchaConstants.ScriptCaptchaChallengeField];
			if (string.IsNullOrWhiteSpace(scriptCaptchaChallenge))
				return false;

			var scriptCaptchaResponse = httpContext.Request.Form[CaptchaConstants.ScriptCaptchaResponseField];
			if (string.IsNullOrWhiteSpace(scriptCaptchaResponse))
				return false;

			scriptCaptchaResponse = new string(scriptCaptchaResponse.Reverse().ToArray());
			return scriptCaptchaChallenge.Equals(scriptCaptchaResponse, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ValidateImageCaptchaResponse(HttpContextBase httpContext)
		{
			var protectedTokenText = httpContext.Request.Form[CaptchaConstants.ImageCaptchaDefaultTokenFieldName];
			var input = httpContext.Request.Form[CaptchaConstants.ImageCaptchaDefaultInputFieldName];

			if (string.IsNullOrWhiteSpace(protectedTokenText) || string.IsNullOrWhiteSpace(input))
				return false;

			try
			{
				var protectedTokenBytes = Convert.FromBase64String(protectedTokenText);
				var tokenBytes = MachineKey.Unprotect(protectedTokenBytes, typeof(CaptchaImageToken).Name);
				var token = CaptchaImageToken.FromBytes(tokenBytes);

				if (!CheckImageTokenTimestamp(token.Timestamp))
					return false;

				return input.Equals(token.ImageContent);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool CheckImageTokenTimestamp(long tokenTimestamp)
		{
			var tokenDateTime = new DateTime(tokenTimestamp);
			var maxDateTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(CaptchaConstants.DefaultValidateImageTimeinSeconds));
			var minDateTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(CaptchaConstants.DefaultValidateImageTimeoutSeconds));

			return tokenDateTime <= maxDateTime && tokenDateTime >= minDateTime;
		}
	}
}