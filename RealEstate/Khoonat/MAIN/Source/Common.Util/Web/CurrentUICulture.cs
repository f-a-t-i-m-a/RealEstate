using System.Threading;

namespace JahanJooy.Common.Util.Web
{
    public static class CurrentUICulture
    {
        public static string Tag
        {
            get { return Thread.CurrentThread.CurrentUICulture.IetfLanguageTag; }
        }

        public static string LanguageCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName; }
        }

        public static string NativeName
        {
            get { return Thread.CurrentThread.CurrentUICulture.NativeName; }
        }

        public static string EnglishName
        {
            get { return Thread.CurrentThread.CurrentUICulture.EnglishName; }
        }

        public static bool IsRightToLeft
        {
            get { return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft; }
        }

        public static bool IsLeftToRight
        {
            get { return !Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft; }
        }

        public static string Direction
        {
            get { return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? "rtl" : "ltr"; }
        }

        public static string ReverseDirection
        {
            get { return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? "ltr" : "rtl"; }
        }

        public static string Far
        {
            get { return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? "left" : "right"; }
        }

        public static string Near
        {
            get { return Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? "right" : "left"; }
        }
    }
}