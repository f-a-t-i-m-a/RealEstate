using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface ICacheValueCopier<TValue>
	{
		TValue Copy(TValue original);
	}
}