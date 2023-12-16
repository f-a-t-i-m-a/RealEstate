using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JahanJooy.Common.Util.Web.Captcha
{
	/// <summary>
	/// HTML Helpers for PoliteCaptcha' spam prevention.
	/// </summary>
	public static class SpamPreventionHtmlHelpers
	{
		/// <summary>
		/// Generates the form fields' HTML for spam prevention. Use inside of an ASP.NET MVC form.
		/// </summary>
		/// <param name="helper">The view's HTML helper.</param>
		/// <returns>The spam prevention form fields' HTML</returns>
		public static IHtmlString ScriptCaptchaToken(this HtmlHelper helper)
		{
			return helper.Hidden(CaptchaConstants.ScriptCaptchaChallengeField, Guid.NewGuid().ToString("N"));
		}

		/// <summary>
		/// Generates the JavaScript required for spam prevention. Requires jQuery.
		/// </summary>
		/// <param name="helper">The view's HTML helper.</param>
		/// <returns>The spam prevention JavaScript.</returns>
		public static IHtmlString ScriptCaptchaScript(this HtmlHelper helper)
		{
			return new HtmlString(
				string.Format(
@"<script>
    $(function () {{
        $('input[name=""{0}""]').each(function() {{
            var form = this.form;
            if (!($(form).children('input[name=""{1}""]').length)) {{
                var response = this.value.split('').reverse().join('');
                $('<input>').attr({{
                    type: 'hidden',
                    name: '{1}',
                    value: response
                }}).appendTo(form);
            }}
        }});
    }});
</script>", CaptchaConstants.ScriptCaptchaChallengeField, CaptchaConstants.ScriptCaptchaResponseField));
		}

		public static IHtmlString CaptchaErrorMessage(this HtmlHelper helper)
		{
			if (helper.ViewData.ModelState.ContainsKey(CaptchaConstants.ModelStateKeyForImageCaptcha))
				return helper.ValidationMessage(CaptchaConstants.ModelStateKeyForImageCaptcha, CaptchaResources.ImageCaptchaNotAcceptable);

			if (helper.ViewData.ModelState.ContainsKey(CaptchaConstants.ModelStateKeyForScriptCaptcha))
				return helper.ValidationMessage(CaptchaConstants.ModelStateKeyForScriptCaptcha, CaptchaResources.ScriptCaptchaNotAcceptable);

			return null;
		}

	}
}
