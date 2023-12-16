using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface ICacheItemLoaderMongo<in TKey, out TValue>
	{
		TValue Load(TKey key);
	}
}