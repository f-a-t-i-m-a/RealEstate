using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;

namespace JahanJooy.RealEstate.Core.Index
{
	[Contract]
	public interface IGlobalSearchEntityMapper<TEntity>
	{
		GlobalSearchIndexItem Map(TEntity entity);
		TEntity Retrieve(GlobalSearchResultItem item);
	}
}