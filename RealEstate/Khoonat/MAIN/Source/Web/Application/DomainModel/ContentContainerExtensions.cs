using Compositional.Composer.Web;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.Common.Util.DomainServiceContracts;

namespace JahanJooy.RealEstate.Web.Application.DomainModel
{
	public static class ContentContainerExtensions
	{
		public static TContent GetContent<TContent>(this IEntityContentContainer<TContent> container) where TContent : class, IEntityContent, new()
		{
			if (container.Content != null)
				return container.Content;

			var serializer = ComposerWebUtil.ComponentContext.GetComponent<IEntityContentSerializer<TContent>>();
			serializer.Deserialize(container);

			if (container.Content == null)
				container.Content = new TContent();

			return container.Content;
		}
	}
}