using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Domain.Property;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.ContentSerialization
{
	[Component]
	public class PropertyListingContentSerializer : IEntityContentSerializer<PropertyListingContent>
	{
		public string Serialize(PropertyListingContent entity)
		{
			return entity.ToJsv();
		}

		public PropertyListingContent Deserialize(string serializedEntity)
		{
			return serializedEntity.FromJsv<PropertyListingContent>();
		}
	}
}