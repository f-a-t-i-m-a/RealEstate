using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class ContractService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveContract(Contract input)
        {
            DbManager.Contract.InsertOneAsync(input).Wait();
            return true;
        }

        public Contract GetContract(string id)
        {
            var filter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(id));
            var contract =
                DbManager.Contract.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return contract;
        }
    }
}