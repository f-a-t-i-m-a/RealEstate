using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.Common.Util.Cache
{
	[Contract]
	public interface ICacheLoader<out TValue>
	{
		IEnumerable<TValue> LoadAll();
	}
}