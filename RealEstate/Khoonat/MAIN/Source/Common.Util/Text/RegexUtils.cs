namespace JahanJooy.Common.Util.Text
{
    public static class RegexUtils
    {
        public static string CreateWholeInputRegex(string regex)
        {
            return "\\A(?:" + regex + ")\\z";
        }
    }
}