using Compositional.Composer;
using Lucene.Net.Documents;

namespace JahanJooy.Common.Util.DomainServiceContracts
{
	[Contract]
	public interface ITwoWayObjectIndexMapper<in TInput, out TOutput> : IObjectIndexMapper<TInput>
	{
		TOutput GetObject(Document document);
	}
}