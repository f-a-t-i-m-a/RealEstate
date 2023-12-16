using System;
using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.Common.Util.Configuration
{
    [Component]
    public class ApplicationSettings : IApplicationSettings
    {
        #region Injected dependencies

        [ComponentPlug]
        public IApplicationSettingsLoader Loader { get; set; }

        #endregion

        #region Caching mechanism

        private Lazy<IDictionary<string, string>> _cachedSettings;
        private readonly object _lockObject = new object();

        [OnCompositionComplete]
        public void InitializeLazyCache()
        {
            lock (_lockObject)
            {
                _cachedSettings = new Lazy<IDictionary<string, string>>(() => Loader.LoadAll());
            }
        }

        #endregion

        #region IApplicationSettings implementation

        public string this[string key]
        {
            get
            {
                lock (_lockObject)
                {
                    string result;
                    if (_cachedSettings.Value.TryGetValue(key, out result))
                        return result;

                    return null;
                }
            }
        }

        public void Reload()
        {
            InitializeLazyCache();
        }

        #endregion
    }
}