using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface ICache<in TKey, out TValue>
	{
		void InvalidateAll();
		TValue this[TKey key] { get; }
	}

    [Contract]
    public interface IItemCache<in TKey, out TValue> : ICache<TKey, TValue>
    {
        void InvalidateItem(TKey key);
        void InvalidateItems(IEnumerable<TKey> keys);
    }
}