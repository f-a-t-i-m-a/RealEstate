using JahanJooy.Common.Util.Text;
using log4net;

namespace JahanJooy.Common.Util.Search
{
	public static class UserSearchQueryUtils
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(UserSearchQueryUtils));

		public static string[] TokenizeUserQuery(string query)
		{
			return query.SplitByWhitespace();
		}
	}
}