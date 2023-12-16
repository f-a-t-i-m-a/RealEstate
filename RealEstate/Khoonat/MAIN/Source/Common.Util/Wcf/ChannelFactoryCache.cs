using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.ServiceModel;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using log4net;

namespace JahanJooy.Common.Util.Wcf
{
    [Contract]
    [Component]
    public class ChannelFactoryCache
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ChannelFactoryCache));

        #region Fields

        private readonly ConcurrentDictionary<Type, ChannelFactory> _factoryCache = 
            new ConcurrentDictionary<Type, ChannelFactory>();

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #endregion

        #region Public methods

        public static void RegisterConfigurationKeys(Type t)
        {
            ApplicationSettingKeys.RegisterKey(GetEndpointAddressConfigurationKey(t));
        }

        public void Use<T>(Action<T> usage)
        {
            var proxy = GetChannelFactory<T>().CreateChannel();
            bool success = false;
            try
            {
                Log.DebugFormat("Starting to use a channel from type {0}", typeof(T));

                usage(proxy);
                ((IClientChannel) proxy).Close();
                success = true;

                Log.DebugFormat("Use of a channel from type {0} completed with success, and the channel is closed.", typeof(T));
            }
            finally
            {
                if (!success)
                {
                    Log.WarnFormat("Use of a channel from type {0} did NOT succeed. Aborting the channel.", typeof (T));
                    ((IClientChannel) proxy).Abort();
                }
            }
        }

        public void Use<T>(string endpointAddress, Action<T> usage)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private helper methods

        private ChannelFactory<T> GetChannelFactory<T>()
        {
            return (ChannelFactory<T>) _factoryCache.GetOrAdd(typeof (T), type => BuildChannelFactory<T>());
        }

        private ChannelFactory BuildChannelFactory<T>()
        {
            var endpointUrl = ApplicationSettings[GetEndpointAddressConfigurationKey(typeof (T))];
            if (string.IsNullOrWhiteSpace(endpointUrl))
                throw new InvalidOperationException("Default Endpoint address for service type " +
                                                    typeof (T).FullName + " is not configured.");

            Log.InfoFormat("Building a new channel factory for type {0} pointing to the endpoint URL '{1}'", typeof(T), endpointUrl);

            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 8 * 1024 * 1024;

            var factory = new ChannelFactory<T>(
                binding,
                new EndpointAddress(endpointUrl));

            return factory;
        }

        private static string GetEndpointAddressConfigurationKey(Type type)
        {
            return GetConfigurationPrefix(type) + ".Url";
        }

        private static string GetConfigurationPrefix(Type t)
        {
            return t.GetCustomAttribute<ServiceContractAttribute>().IfNotNull(a => a.ConfigurationName, t.FullName);
        }

        #endregion
    }
}