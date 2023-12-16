using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Impl.Index.Base;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Component]
	public class GlobalSearchIndex : ObjectIndexBase<GlobalSearchIndexItem>, IGlobalSearchIndex
	{
		private const string IndexIDString = "Global";

		#region Initialization

		static GlobalSearchIndex()
		{
			LuceneIndexManager.RegisterConfigurationKeys(IndexIDString);
		}

		[OnCompositionComplete]
		public void OnCompositionComplete()
		{
			IndexManager.InitializeIndex(IndexIDString);
		}

		#endregion

		public override string IndexID
		{
			get { return IndexIDString; }
		}

		protected override Query GetIdentityQuery(GlobalSearchIndexItem obj)
		{
			var result = new BooleanQuery
			             {
				             {NumericRangeQuery.NewLongRange("ID", obj.ID, obj.ID, true, true), Occur.MUST},
				             {new TermQuery(new Term("Type", obj.Type.ToString())), Occur.MUST}
			             };

			return result;
		}

		public PagedList<GlobalSearchResultItem> RunQuery(Query query, int startIndex, int pageSize)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			if (startIndex < 0)
				throw new ArgumentException("StartIndex cannot be negative");
			if (pageSize <= 0)
				throw new ArgumentException("PageSize cannot be negative or zero");

			using (var searcher = IndexManager.AcquireSearcher(IndexIDString))
			{
				var topDocs = searcher.Search(query, startIndex + pageSize);
				var result = PagedList<GlobalSearchResultItem>.BuildUsingStartIndex(topDocs.TotalHits, pageSize, startIndex);
				result.PageItems = new List<GlobalSearchResultItem>();

				var indexMap = Composer.GetComponent<ITwoWayObjectIndexMapper<GlobalSearchIndexItem, GlobalSearchResultItem>>();

				foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
				{
					var doc = searcher.Doc(scoreDoc.Doc);
					var obj = indexMap.GetObject(doc);
					result.PageItems.Add(obj);
				}
				return result;
			}
		}
	}
}