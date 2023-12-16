using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using JahanJooy.Common.Util.Security;

namespace JahanJooy.Common.Util.Web.Captcha.Image
{
	public class CaptchaImage
	{
		private int _height;
		private int _width;
		private string _fontFamilyName;

		/// <summary>
		/// Font family to use when drawing the Captcha text. If no font is provided, a random font will be chosen from the font whitelist for each character.
		/// </summary>
		public string Font
		{
			get { return _fontFamilyName; }
			set
			{
				Font font = null;
				try
				{
					font = new Font(value, 12f);
					_fontFamilyName = value;
				}
				catch (Exception)
				{
					_fontFamilyName = FontFamily.GenericSerif.Name;
				}
				finally
				{
					if (font != null)
						font.Dispose();
				}
			}
		}

		/// <summary>
		/// Amount of random warping to apply to the Captcha text.
		/// </summary>
		public FontWarpFactor FontWarp { get; set; }

		/// <summary>
		/// Amount of background noise to apply to the Captcha image.
		/// </summary>
		public BackgroundNoiseLevel BackgroundNoise { get; set; }

		public LineNoiseLevel LineNoise { get; set; }

		/// <summary>
		/// Specifies the Captcha text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Width of Captcha image to generate, in pixels
		/// </summary>
		public int Width
		{
			get { return _width; }
			set
			{
				if (value <= 60)
					throw new ArgumentException("Width must be greater than 60.");
				_width = value;
			}
		}

		/// <summary>
		/// Height of Captcha image to generate, in pixels
		/// </summary>
		public int Height
		{
			get { return _height; }
			set
			{
				if (value <= 30)
					throw new ArgumentException("Height must be greater than 30.");
				_height = value;
			}
		}

		/// <summary>
		/// A semicolon-delimited list of valid fonts to use when no font is provided.
		/// 
		/// </summary>
		public string FontWhitelist { get; set; }

		/// <summary>
		/// Background color for the captcha image
		/// 
		/// </summary>
		public Color BackColor { get; set; }

		/// <summary>
		/// Color of captcha text
		/// 
		/// </summary>
		public Color FontColor { get; set; }

		/// <summary>
		/// Color for dots in the background noise
		/// 
		/// </summary>
		public Color NoiseColor { get; set; }

		/// <summary>
		/// Color for the background lines of the captcha image
		/// 
		/// </summary>
		public Color LineColor { get; set; }

		public CaptchaImage()
		{
			LineColor = Color.Black;
			NoiseColor = Color.Black;
			FontColor = Color.Black;
			BackColor = Color.White;
			FontWarp = FontWarpFactor.Low;
			BackgroundNoise = BackgroundNoiseLevel.Low;
			LineNoise = LineNoiseLevel.None;
			_width = 180;
			_height = 50;
			_fontFamilyName = "";
			FontWhitelist =
				"arial;arial black;comic sans ms;courier new;estrangelo edessa;franklin gothic medium;georgia;lucida console;lucida sans unicode;mangal;microsoft sans serif;palatino linotype;sylfaen;tahoma;times new roman;trebuchet ms;verdana";
		}

		/// <summary>
		/// Forces a new Captcha image to be generated using current property value settings.
		/// 
		/// </summary>
		public Bitmap RenderImage()
		{
			return GenerateImagePrivate();
		}

		/// <summary>
		/// Returns a random font family from the font whitelist
		/// 
		/// </summary>
		private string RandomFontFamily()
		{
			string[] strArray = FontWhitelist.Split(new[]
				                                        {
					                                        ';'
				                                        });
			
			return strArray[CryptoRandomNumberUtil.GetInt32(0, strArray.Length - 1)];
		}

		/// <summary>
		/// Returns a random point within the specified x and y ranges
		/// </summary>
		private PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
		{
			return new PointF(CryptoRandomNumberUtil.GetInt32(xmin, xmax), CryptoRandomNumberUtil.GetInt32(ymin, ymax));
		}

		/// <summary>
		/// Returns a random point within the specified rectangle
		/// </summary>
		private PointF RandomPoint(Rectangle rect)
		{
			return RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom);
		}

		/// <summary>
		/// Returns a GraphicsPath containing the specified string and font
		/// </summary>
		private GraphicsPath TextPath(string s, Font f, Rectangle r)
		{
			var format = new StringFormat
				             {
					             Alignment = StringAlignment.Near,
					             LineAlignment = StringAlignment.Near
				             };
			var graphicsPath = new GraphicsPath();
			graphicsPath.AddString(s, f.FontFamily, (int) f.Style, f.Size, r, format);
			return graphicsPath;
		}

		/// <summary>
		/// Returns the CAPTCHA font in an appropriate size
		/// </summary>
		private Font GetFont()
		{
			float emSize = 0.0f;
			string familyName = _fontFamilyName;
			if (familyName == string.Empty)
				familyName = RandomFontFamily();
			switch (FontWarp)
			{
				case FontWarpFactor.None:
					emSize = Convert.ToInt32(_height*0.7);
					break;
				case FontWarpFactor.Low:
					emSize = Convert.ToInt32(_height*0.8);
					break;
				case FontWarpFactor.Medium:
					emSize = Convert.ToInt32(_height*0.85);
					break;
				case FontWarpFactor.High:
					emSize = Convert.ToInt32(_height*0.9);
					break;
				case FontWarpFactor.Extreme:
					emSize = Convert.ToInt32(_height*0.95);
					break;
			}
			return new Font(familyName, emSize, FontStyle.Bold);
		}

		/// <summary>
		/// Renders the CAPTCHA image
		/// 
		/// </summary>
		private Bitmap GenerateImagePrivate()
		{
			var bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
			using (Graphics graphics1 = Graphics.FromImage(bitmap))
			{
				graphics1.SmoothingMode = SmoothingMode.AntiAlias;
				var rect = new Rectangle(0, 0, _width, _height);
				Brush brush1;
				using (brush1 = new SolidBrush(BackColor))
					graphics1.FillRectangle(brush1, rect);
				int num1 = 0;
				double num2 = (double)_width/Text.Length;
				Brush brush2;
				using (brush2 = new SolidBrush(FontColor))
				{
					foreach (char ch in Text)
					{
						Font font;
						using (font = GetFont())
						{
							var rectangle = new Rectangle(Convert.ToInt32(num1*num2), 0, Convert.ToInt32(num2), _height);
							using (GraphicsPath graphicsPath = TextPath(ch.ToString(CultureInfo.InvariantCulture), font, rectangle))
							{
								WarpText(graphicsPath, rectangle);
								graphics1.FillPath(brush2, graphicsPath);
							}
						}
						++num1;
					}
				}
				AddNoise(graphics1, rect);
				AddLine(graphics1, rect);
			}
			return bitmap;
		}

		/// <summary>
		/// Warp the provided text GraphicsPath by a variable amount
		/// 
		/// </summary>
		private void WarpText(GraphicsPath textPath, Rectangle rect)
		{
			float num1 = 1f;
			float num2 = 1f;
			switch (FontWarp)
			{
				case FontWarpFactor.None:
					return;
				case FontWarpFactor.Low:
					num1 = 6f;
					num2 = 1f;
					break;
				case FontWarpFactor.Medium:
					num1 = 5f;
					num2 = 1.3f;
					break;
				case FontWarpFactor.High:
					num1 = 4.5f;
					num2 = 1.4f;
					break;
				case FontWarpFactor.Extreme:
					num1 = 4f;
					num2 = 1.5f;
					break;
			}

			var srcRect = new RectangleF(Convert.ToSingle(rect.Left), 0.0f, Convert.ToSingle(rect.Width), rect.Height);
			int num3 = Convert.ToInt32(rect.Height/num1);
			int num4 = Convert.ToInt32(rect.Width/num1);
			int xmin = rect.Left - Convert.ToInt32(num4*num2);
			int ymin = rect.Top - Convert.ToInt32(num3*num2);
			int xmax = rect.Left + rect.Width + Convert.ToInt32(num4*num2);
			int ymax = rect.Top + rect.Height + Convert.ToInt32(num3*num2);
			if (xmin < 0)
				xmin = 0;
			if (ymin < 0)
				ymin = 0;
			if (xmax > Width)
				xmax = Width;
			if (ymax > Height)
				ymax = Height;
			var destPoints = new[]
				                 {
					                 RandomPoint(xmin, xmin + num4, ymin, ymin + num3),
					                 RandomPoint(xmax - num4, xmax, ymin, ymin + num3),
					                 RandomPoint(xmin, xmin + num4, ymax - num3, ymax),
					                 RandomPoint(xmax - num4, xmax, ymax - num3, ymax)
				                 };
			var matrix = new Matrix();
			matrix.Translate(0.0f, 0.0f);
			textPath.Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.0f);
		}

		/// <summary>
		/// Add a variable level of graphic noise to the image
		/// 
		/// </summary>
		private void AddNoise(Graphics graphics1, Rectangle rect)
		{
			int num1 = 0;
			int num2 = 0;
			switch (BackgroundNoise)
			{
				case BackgroundNoiseLevel.None:
					return;
				case BackgroundNoiseLevel.Low:
					num1 = 30;
					num2 = 40;
					break;
				case BackgroundNoiseLevel.Medium:
					num1 = 18;
					num2 = 40;
					break;
				case BackgroundNoiseLevel.High:
					num1 = 16;
					num2 = 39;
					break;
				case BackgroundNoiseLevel.Extreme:
					num1 = 12;
					num2 = 38;
					break;
			}
			using (var solidBrush = new SolidBrush(NoiseColor))
			{
				int maxValue = Convert.ToInt32(Math.Max(rect.Width, rect.Height)/num2);
				for (int index = 0; index <= Convert.ToInt32(rect.Width*rect.Height/num1); ++index)
					graphics1.FillEllipse(solidBrush, 
						CryptoRandomNumberUtil.GetInt32(0, rect.Width),
						CryptoRandomNumberUtil.GetInt32(0, rect.Height), 
						CryptoRandomNumberUtil.GetInt32(0, maxValue), 
						CryptoRandomNumberUtil.GetInt32(0, maxValue));
			}
		}

		/// <summary>
		/// Add variable level of curved lines to the image
		/// 
		/// </summary>
		private void AddLine(Graphics graphics1, Rectangle rect)
		{
			int num1 = 0;
			float width = 1f;
			int num2 = 0;
			switch (LineNoise)
			{
				case LineNoiseLevel.None:
					return;
				case LineNoiseLevel.Low:
					num1 = 4;
					width = Convert.ToSingle(_height/31.25);
					num2 = 1;
					break;
				case LineNoiseLevel.Medium:
					num1 = 5;
					width = Convert.ToSingle(_height/27.7777);
					num2 = 1;
					break;
				case LineNoiseLevel.High:
					num1 = 3;
					width = Convert.ToSingle(_height/25);
					num2 = 2;
					break;
				case LineNoiseLevel.Extreme:
					num1 = 3;
					width = Convert.ToSingle(_height/22.7272);
					num2 = 3;
					break;
			}
			var points = new PointF[num1 + 1];
			using (var pen = new Pen(LineColor, width))
			{
				for (int index1 = 1; index1 <= num2; ++index1)
				{
					for (int index2 = 0; index2 <= num1; ++index2)
						points[index2] = RandomPoint(rect);
					graphics1.DrawCurve(pen, points, 1.75f);
				}
			}
		}

		/// <summary>
		/// Amount of random font warping to apply to rendered text
		/// 
		/// </summary>
		public enum FontWarpFactor
		{
			None,
			Low,
			Medium,
			High,
			Extreme,
		}

		/// <summary>
		/// Amount of background noise to add to rendered image
		/// 
		/// </summary>
		public enum BackgroundNoiseLevel
		{
			None,
			Low,
			Medium,
			High,
			Extreme,
		}

		/// <summary>
		/// Amount of curved line noise to add to rendered image
		/// 
		/// </summary>
		public enum LineNoiseLevel
		{
			None,
			Low,
			Medium,
			High,
			Extreme,
		}

		/// <summary>
		/// Arithmetic operation to perform in formula
		/// 
		/// </summary>
		public enum ArithmeticOperation
		{
			Random,
			Addition,
			Substraction,
		}
	}
}