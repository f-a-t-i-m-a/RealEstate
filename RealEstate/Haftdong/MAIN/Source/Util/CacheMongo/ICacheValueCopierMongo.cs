using Compositional.Composer;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo
{
	[Contract]
	public interface ICacheValueCopierMongo<TValue>
	{
		TValue Copy(TValue original);
	}
}