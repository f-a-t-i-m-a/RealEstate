using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class ConfigurationDataItemService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveConfigurationDataItem(ConfigurationDataItem input)
        {
            DbManager.ConfigurationDataItem.InsertOneAsync(input).Wait();
            return true;
        }

        public ConfigurationDataItem GetConfigurationDataItem(string id)
        {
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("_id", ObjectId.Parse(id));
            var item =
                DbManager.ConfigurationDataItem.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return item;
        }
    }
}