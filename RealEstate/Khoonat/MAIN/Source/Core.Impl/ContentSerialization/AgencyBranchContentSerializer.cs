using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Domain.Directory;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.ContentSerialization
{
	[Component]
	public class AgencyBranchContentSerializer : IEntityContentSerializer<AgencyBranchContent>
	{
		public string Serialize(AgencyBranchContent entity)
		{
			return entity.ToJsv();
		}

		public AgencyBranchContent Deserialize(string serializedEntity)
		{
			return serializedEntity.FromJsv<AgencyBranchContent>();
		}
	}
}