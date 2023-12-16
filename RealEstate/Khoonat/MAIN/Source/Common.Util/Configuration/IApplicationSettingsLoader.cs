using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.Common.Util.Configuration
{
    [Contract]
    public interface IApplicationSettingsLoader
    {
        string Load(string key);
        IDictionary<string, string> LoadAll();
    }
}