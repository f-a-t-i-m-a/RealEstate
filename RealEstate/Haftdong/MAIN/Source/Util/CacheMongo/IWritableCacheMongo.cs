using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface IWritableCacheMongo<in TKey, TValue> : ICacheMongo<TKey, TValue>
	{
		TValue Put(TKey key, TValue value);
	}
}