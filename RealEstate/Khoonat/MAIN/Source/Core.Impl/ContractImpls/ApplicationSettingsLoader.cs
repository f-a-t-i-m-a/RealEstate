using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Impl.Data;

namespace JahanJooy.RealEstate.Core.Impl.ContractImpls
{
    [Component]
    public class ApplicationSettingsLoader : IApplicationSettingsLoader
    {
        public string Load(string key)
        {
            using (var db = new Db())
            {
                return db.ConfigurationDataItems
                    .SingleOrDefault(i => i.Identifier == key)
                    .IfNotNull(i => i.Value);
            }
        }

        public IDictionary<string, string> LoadAll()
        {
            using (var db = new Db())
            {
                return db.ConfigurationDataItems
                    .ToDictionary(i => i.Identifier, i => i.Value);
            }
        }
    }
}