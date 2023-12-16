using System.Collections.Generic;
using JahanJooy.Common.Util.Collections;
using Lucene.Net.Search;

namespace JahanJooy.Common.Util.Search
{
	public static class LuceneExtensions
	{
		public static void AddAll(this BooleanQuery baseQuery, IEnumerable<Query> queries, Occur occur)
		{
			queries.ForEach(q => baseQuery.Add(q, occur));
		}
	}
}