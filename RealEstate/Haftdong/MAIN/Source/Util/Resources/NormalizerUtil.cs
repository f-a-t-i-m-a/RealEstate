using System;
// ReSharper disable InconsistentNaming

namespace JahanJooy.RealEstateAgency.Util.Resources
{
    public static class NormalizerUtil
    {
//        public const char YEH = '\uD98A';
//        public const char FARSI_YEH = '\uDB8C';
//        public const char YEH_BARREE = '\uDB92';
//        public const char KEHEH = '\uDAA9';
//        public const char KAF = '\uD983';
//        public const char HAMZA_ABOVE = '\uD994';
//        public const char HEH_YEH = '\uDB80';
//        public const char HEH_GOAL = '\uDB81';
//        public const char HEH = '\uD987';

        public const char YEH = 'ي';
        public const char FARSI_YEH = 'ی';
        public const char YEH_BARREE = 'ے';
        public const char KEHEH = 'ک';
        public const char KAF = 'ك';
        public const char HAMZA_ABOVE = 'ٔ';
        public const char HEH_YEH = 'ۀ';
        public const char HEH_GOAL = 'ہ';
        public const char HEH = 'ه';

        public static int NormalizeToPersian(char[] s, int len)
        {
            for (int pos = 0; pos < len; ++pos)
            {
                switch (s[pos])
                {
                    case HEH_YEH:
                    case HEH_GOAL:
                        s[pos] = HEH;
                        break;
                    case YEH:
                    case YEH_BARREE:
                        s[pos] = FARSI_YEH;
                        break;
                    case HAMZA_ABOVE:
                        len = Delete(s, pos, len);
                        --pos;
                        break;
                    case KAF:
                        s[pos] = KEHEH;
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