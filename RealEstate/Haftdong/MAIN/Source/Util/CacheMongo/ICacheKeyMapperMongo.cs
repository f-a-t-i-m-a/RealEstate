using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface ICacheKeyMapperMongo<out TKey, in TValue>
	{
		TKey MapKey(TValue value);
	}
}