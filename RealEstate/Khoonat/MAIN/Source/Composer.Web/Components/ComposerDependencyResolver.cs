using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer.Web.Contracts;

namespace Compositional.Composer.Web.Components
{
	[Component]
	public class ComposerDependencyResolver : IDependencyResolverContract
	{
		private readonly IDependencyResolver _baseResolver;

		[ComponentPlug]
		public IComposer Composer { get; set; }

		public ComposerDependencyResolver()
		{
			_baseResolver = DependencyResolver.Current;
		}

		#region Implementation of IDependencyResolver

		public object GetService(Type serviceType)
		{
			object result = null;

			if (ComponentContextUtils.HasContractAttribute(serviceType))
				result = Composer.GetComponent(serviceType);

			if (result == null)
			{
				result = _baseResolver.GetService(serviceType);

				if (result != null)
					Composer.InitializePlugs(result, result.GetType());
			}

			return result;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			var baseResult = _baseResolver.GetServices(serviceType);

			baseResult.ToList().ForEach(o => Composer.InitializePlugs(o, o.GetType()));

			if (ComponentContextUtils.HasContractAttribute(serviceType))
				return Composer.GetAllComponents(serviceType).Concat(baseResult);

			return baseResult;
		}

		#endregion
	}
}