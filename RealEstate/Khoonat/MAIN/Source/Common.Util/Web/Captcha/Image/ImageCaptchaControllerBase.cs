using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using JahanJooy.Common.Util.Security;

namespace JahanJooy.Common.Util.Web.Captcha.Image
{
	public abstract class ImageCaptchaControllerBase : Controller
	{
		protected virtual string CaptchaCharacters
		{
			get { return CaptchaConstants.Digits; }
		}

		protected virtual int CaptchaLength
		{
			get { return CaptchaConstants.DefaultCaptchaLength; }
		}

		public ActionResult ViewImage()
		{
			var token = new CaptchaImageToken(GenerateRandomText());
			var protectedTokenBytes = MachineKey.Protect(token.ToBytes(), typeof(CaptchaImageToken).Name);
			var protectedTokenString = Convert.ToBase64String(protectedTokenBytes);

			return PartialView((object)protectedTokenString);
		}

		public ActionResult GetImageBytes(string code)
		{
			try
			{
				var protectedTokenBytes = Convert.FromBase64String(code);
				var tokenBytes = MachineKey.Unprotect(protectedTokenBytes, typeof(CaptchaImageToken).Name);
				var token = CaptchaImageToken.FromBytes(tokenBytes);

				if (!CheckGetImageBytesTimeout(token.Timestamp))
					return InvalidCaptchaParamImage();

				var image = new CaptchaImage();
				ApplyCaptchaImageProperties(image);

				image.Text = token.ImageContent;

				var resultStream = new MemoryStream();
				using (Bitmap bitmap = image.RenderImage())
					bitmap.Save(resultStream, ImageFormat.Jpeg);

				return File(resultStream.ToArray(), "image/jpeg");
			}
			catch (Exception)
			{
				return InvalidCaptchaParamImage();
			}
		}

		protected virtual ActionResult InvalidCaptchaParamImage()
		{
			// TODO: return an actual image that shows an error
			return File(new byte[0], "image/png");
		}

		protected virtual string GenerateRandomText()
		{
			string referenceChars = CaptchaCharacters;
			int length = CaptchaLength;

			var stringBuilder = new StringBuilder(length);
			for (int index = 0; index <= length - 1; ++index)
				stringBuilder.Append(referenceChars.Substring(CryptoRandomNumberUtil.GetInt32(0, referenceChars.Length - 1), 1));
			return stringBuilder.ToString();
		}

		protected virtual bool CheckGetImageBytesTimeout(long tokenTimestamp)
		{
			var tokenDateTime = new DateTime(tokenTimestamp);
			return tokenDateTime <= DateTime.Now && tokenDateTime >= DateTime.Now.Subtract(TimeSpan.FromSeconds(CaptchaConstants.DefaultGetImageBytesTimeoutSeconds));
		}

		protected virtual void ApplyCaptchaImageProperties(CaptchaImage image)
		{
			image.BackgroundNoise = CaptchaImage.BackgroundNoiseLevel.High;
			image.FontWarp = CaptchaImage.FontWarpFactor.Low;
			image.LineColor = Color.DarkGray;
			image.LineNoise = CaptchaImage.LineNoiseLevel.Low;
			image.BackColor = Color.LightGray;
		}
	}
}