using System;

namespace JahanJooy.Common.Util.Text
{
	/// <summary>
	/// Source code copied from PersianNormalizer class within Lucene.Net
	/// </summary>
	public class PersianCharacterNormalizer
	{
		public const char YEH = 'ي';
		public const char FARSI_YEH = 'ی';
		public const char YEH_BARREE = 'ے';
		public const char KEHEH = 'ک';
		public const char KAF = 'ك';
		public const char HAMZA_ABOVE = 'ٔ';
		public const char HEH_YEH = 'ۀ';
		public const char HEH_GOAL = 'ہ';
		public const char HEH = 'ه';

		public static int Normalize(char[] s, int len)
		{
			for (int pos = 0; pos < len; ++pos)
			{
				switch (s[pos])
				{
					case 'ۀ':
					case 'ہ':
						s[pos] = 'ه';
						break;
					case 'ی':
					case 'ے':
						s[pos] = 'ي';
						break;
					case 'ٔ':
						len = Delete(s, pos, len);
						--pos;
						break;
					case 'ک':
						s[pos] = 'ك';
						break;
				}
			}
			return len;
		}

		private static int Delete(char[] s, int pos, int len)
		{
			if (pos < len)
				Array.Copy(s, pos + 1, s, pos, len - pos - 1);
			return len - 1;
		}
	}
}