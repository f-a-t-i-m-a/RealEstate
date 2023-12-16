using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class PropertyService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveProperty(Property input)
        {
            DbManager.Property.InsertOneAsync(input).Wait();
            return true;
        }

        public Property GetProperty(string id)
        {
            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));
            var property =
                DbManager.Property.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return property;
        }
    }
}