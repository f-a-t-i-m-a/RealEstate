using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface ICacheMongo<in TKey, out TValue>
	{
		void InvalidateAll();
		TValue this[TKey key] { get; }
	}

    [Contract]
    public interface IItemCacheMongo<in TKey, out TValue> : ICacheMongo<TKey, TValue>
    {
        void InvalidateItem(TKey key);
        void InvalidateItems(IEnumerable<TKey> keys);
    }
}