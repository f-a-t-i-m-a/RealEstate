using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class SupplyService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveSupply(Supply input)
        {
            DbManager.Supply.InsertOneAsync(input).Wait();
            return true;
        }

        public Supply GetSupply(string id)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply =
                DbManager.Supply.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return supply;
        }
    }
}