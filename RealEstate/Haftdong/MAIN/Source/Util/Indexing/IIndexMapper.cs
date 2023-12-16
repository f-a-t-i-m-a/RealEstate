using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Contract]
    public interface IIndexMapper<in TEntity, out TEntityIE>
    {
        TEntityIE Map(TEntity entity);
    }
}