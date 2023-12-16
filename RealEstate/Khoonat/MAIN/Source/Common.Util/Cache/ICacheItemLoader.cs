using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface ICacheItemLoader<in TKey, out TValue>
	{
		TValue Load(TKey key);
	}
}