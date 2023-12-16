using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Index
{
	[Contract]
	public interface IGlobalSearchIndex : IObjectIndex<GlobalSearchIndexItem>
	{
		PagedList<GlobalSearchResultItem> RunQuery(Query query, int startIndex, int pageSize);
	}
}