using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Util.Configuration
{
    [Component]
    public class ApplicationSettingsLoader : IApplicationSettingsLoader
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public string Load(string key)
        {
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("Identifier", key);
            var result = DbManager.ConfigurationDataItem.Find(filter).SingleOrDefaultAsync().Result;
            if (result == null)
                return "";
            return result.Value;
        }

        public IDictionary<string, string> LoadAll()
        {
            var filter = new BsonDocument();
            var result = DbManager.ConfigurationDataItem.Find(filter).ToListAsync().Result;
            var output = result.ToDictionary(r => r.Identifier, r => r.Value);
            return output;
        }
    }
}