using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IGlobalSearchService
	{
		GlobalSearchResult RunSearch(GlobalSearchCriteria criteria, int startIndex, int pageSize);
	}
}