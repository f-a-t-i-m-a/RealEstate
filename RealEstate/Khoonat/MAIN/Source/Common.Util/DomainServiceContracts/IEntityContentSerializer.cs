using Compositional.Composer;
using JahanJooy.Common.Util.DomainModel;
using ServiceStack;

namespace JahanJooy.Common.Util.DomainServiceContracts
{
	[Contract]
	public interface IEntityContentSerializer<TContent> where TContent : class, IEntityContent
	{
		string Serialize(TContent entity);
		TContent Deserialize(string serializedEntity);
	}

	public static class EntityContainerSerializerExtentions
	{
		public static void Serialize<TContent>(this IEntityContentSerializer<TContent> serializer, IEntityContentContainer<TContent> container)
			where TContent : class, IEntityContent
		{
			container.ContentString = container.Content == null ? string.Empty : serializer.Serialize(container.Content);
		}

		public static void Deserialize<TContent>(this IEntityContentSerializer<TContent> serializer, IEntityContentContainer<TContent> container)
			where TContent : class, IEntityContent, new()
		{
			container.Content = container.ContentString.IsNullOrEmpty() ? new TContent() : serializer.Deserialize(container.ContentString);
		}

		public static void DeserializeIfNeeded<TContent>(this IEntityContentSerializer<TContent> serializer, IEntityContentContainer<TContent> container)
			where TContent : class, IEntityContent, new()
		{
			if (container.Content == null)
				serializer.Deserialize(container);
		}
	}
}