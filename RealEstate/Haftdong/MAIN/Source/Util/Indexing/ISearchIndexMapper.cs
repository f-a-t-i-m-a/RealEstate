using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Contract]
    public interface ISearchIndexMapper<in TEntity, out TSearchEntityIE>
    {
        TSearchEntityIE SearchMap(TEntity entity);
    }
}