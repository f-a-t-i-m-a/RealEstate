using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Domain.Property;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.ContentSerialization
{
	[Component]
	public class PropertyRequestContentSerializer : IEntityContentSerializer<PropertyRequestContent>
	{
		public string Serialize(PropertyRequestContent entity)
		{
			return entity.ToJsv();
		}

		public PropertyRequestContent Deserialize(string serializedEntity)
		{
			return serializedEntity.FromJsv<PropertyRequestContent>();
		}
	}
}