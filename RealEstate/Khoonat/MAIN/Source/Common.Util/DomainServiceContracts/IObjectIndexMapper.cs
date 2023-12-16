using Compositional.Composer;
using Lucene.Net.Documents;

namespace JahanJooy.Common.Util.DomainServiceContracts
{
	[Contract]
	public interface IObjectIndexMapper<in T>
	{
		void PopulateDocument(T obj);
		Document GetDocument();

		bool DocumentReady { get; }
	}
}
