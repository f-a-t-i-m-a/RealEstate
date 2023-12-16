using System;
using System.Collections.Generic;
using System.Text;

namespace JahanJooy.Common.Util.Web.Captcha.Image
{
	public class CaptchaImageToken
	{
		public string ImageContent { get; private set; }
		public long Timestamp { get; private set; }

		private CaptchaImageToken()
		{
		}

		public CaptchaImageToken(string imageContent)
		{
			if (string.IsNullOrWhiteSpace(imageContent))
				throw new ArgumentNullException("imageContent");

			ImageContent = imageContent;
			Timestamp = DateTime.Now.Ticks;
		}

		public byte[] ToBytes()
		{
			var result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(Timestamp));
			result.AddRange(Encoding.UTF8.GetBytes(ImageContent));

			return result.ToArray();
		}

		public static CaptchaImageToken FromBytes(byte[] bytes)
		{
			if (bytes == null || bytes.Length <= 8)
				return null;

			var result = new CaptchaImageToken();
			result.Timestamp = BitConverter.ToInt64(bytes, 0);
			result.ImageContent = Encoding.UTF8.GetString(bytes, 8, bytes.Length - 8);

			return result;
		}
	}
}