namespace JahanJooy.Common.Util.Web.Captcha
{
	public static class CaptchaConstants
	{
		public const string Digits = "0123456789";
		public const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
		public const string DigitsAndUpperCaseLetters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string DigitsAndLowerCaseLetters = "0123456789abcdefghijklmnopqrstuvwxyz";
		public const string DigitsAndAllLetters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public const int DefaultCaptchaLength = 6;
		public const int DefaultGetImageBytesTimeoutSeconds = 10;
		public const int DefaultValidateImageTimeoutSeconds = 240;
		public const int DefaultValidateImageTimeinSeconds = 3;

		public const string ModelStateKeyForScriptCaptcha = "ScriptCaptchaError";
		public const string ModelStateKeyForImageCaptcha = "ImageCaptchaError";

		public const string ScriptCaptchaChallengeField = "jscaptcha_challenge";
		public const string ScriptCaptchaResponseField = "jscaptcha_response";
		public const string ImageCaptchaDefaultTokenFieldName = "image_captcha_token";
		public const string ImageCaptchaDefaultInputFieldName = "image_captcha_input";

	}
}