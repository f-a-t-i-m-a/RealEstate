using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface ICacheKeyMapper<out TKey, in TValue>
	{
		TKey MapKey(TValue value);
	}
}