using System.Linq;
using System.Web;

namespace JahanJooy.Common.Util.EF
{
	public static class QueryExtensions
	{
		public static IQueryable<T> TraceSql<T>(this IQueryable<T> query)
		{
			var sql = query.ToString();

			// (view by visiting trace.axd within your site)
			HttpContext.Current.Trace.Write("sql", sql);

			return query;
		}
	}
}