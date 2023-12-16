using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Domain.Directory;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.ContentSerialization
{
	[Component]
	public class AgencyContentSerializer : IEntityContentSerializer<AgencyContent>
	{
		public string Serialize(AgencyContent entity)
		{
			return entity.ToJsv();
		}

		public AgencyContent Deserialize(string serializedEntity)
		{
			return serializedEntity.FromJsv<AgencyContent>();
		}
	}
}