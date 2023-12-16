using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using Compositional.Composer.Emitter;
using Compositional.Composer.Interceptor;
using Compositional.Composer.Utility;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.ServiceInterceptor
{
	public class ServiceTransactionCompositionListener : ICompositionListener
	{
		[ComponentPlug]
		public IComposer Composer { get; set; }

		private readonly string[] _serviceInterfaceNamespaces;
		private readonly Assembly _serviceInterfaceAssembly;

		public ServiceTransactionCompositionListener()
		{
		    _serviceInterfaceNamespaces = new[]
		                                  {
		                                      typeof (IAuthenticationService).Namespace, 
                                              typeof (IUserBalanceService).Namespace,
                                              typeof (ISponsoredPropertyService).Namespace,
		                                  };
		    _serviceInterfaceAssembly = typeof (IAuthenticationService).Assembly;
		}

		public void OnComponentCreated(ContractIdentity identity, IComponentFactory componentFactory, Type componentTargetType,
		                               ref object componentInstance, object originalInstance)
		{
			// Determine if the component instance is a service implementation. If not, do nothing.

			if (!identity.Type.IsInterface)
				return;

			if (!_serviceInterfaceNamespaces.Contains(identity.Type.Namespace))
				return;

			if (!_serviceInterfaceAssembly.Equals(identity.Type.Assembly))
				return;

			if (!identity.Type.Name.EndsWith("Service"))
				return;

			// The created component is a service. Proxy it with transaction interceptor.

			var interceptor = Composer.GetComponent<ServiceTransactionInterceptor>();
			var handler = new InterceptingAdapterEmittedTypeHanlder(componentInstance, interceptor);

			var classEmitter = Composer.GetComponent<IClassEmitter>();
			var adaptor = classEmitter.EmitInterfaceInstance(handler, identity.Type);

			componentInstance = adaptor;
		}

		public void OnComponentComposed(ContractIdentity identity, IEnumerable<InitializationPointSpecification> initializationPoints,
		                                IEnumerable<object> initializationPointResults, Type componentTargetType,
		                                object componentInstance, object originalInstance)
		{
			// Do nothing
		}

		public void OnComponentRetrieved(ContractIdentity identity, IComponentFactory componentFactory, Type componentTargetType, ref object componentInstance, object originalInstance)
		{
			// Do nothing
		}
	}
}