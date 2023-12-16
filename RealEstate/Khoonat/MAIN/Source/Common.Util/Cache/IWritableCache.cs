using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface IWritableCache<in TKey, TValue> : ICache<TKey, TValue>
	{
		TValue Put(TKey key, TValue value);
	}
}