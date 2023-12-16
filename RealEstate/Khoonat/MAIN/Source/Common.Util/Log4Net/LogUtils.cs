using System.Text.RegularExpressions;

namespace JahanJooy.Common.Util.Log4Net
{
	public static class LogUtils
	{
		private static readonly Regex InvalidInputCharactersRegex = new Regex(@"[\x00-\x1F]");

		public static string SanitizeUserInput(string input)
		{
			input = input ?? "<NULL>";
			return InvalidInputCharactersRegex.Replace(input, "-");
		}
	}
}