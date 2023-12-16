using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface ICacheLoaderMongo<out TValue>
	{
		IEnumerable<TValue> LoadAll();
	}
}