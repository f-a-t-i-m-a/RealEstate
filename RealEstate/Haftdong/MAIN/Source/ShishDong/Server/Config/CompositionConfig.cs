using System.Linq;
using System.Web.Http;
using Compositional.Composer;
using Compositional.Composer.Web;
using JahanJooy.Common.Util.Components;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Config
{
	public static class CompositionConfig
	{
		public static IComposer Setup(HttpConfiguration configuration)
		{
			ComposerWebUtil.Setup();
			var componentContext = ComposerWebUtil.ComponentContext;

			// Instantiate eager components
			// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			ComposerWebUtil.ComponentContext.GetAllComponents<IEagerComponent>().ToList();
			// ReSharper restore ReturnValueOfPureMethodIsNotUsed

			return componentContext;
		}
	}
}