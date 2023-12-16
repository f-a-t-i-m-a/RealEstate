using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class RequestService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveRequest(Request input)
        {
            DbManager.Request.InsertOneAsync(input).Wait();
            return true;
        }

        public Request GetRequest(string id)
        {
            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request =
                DbManager.Request.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return request;
        }
    }
}