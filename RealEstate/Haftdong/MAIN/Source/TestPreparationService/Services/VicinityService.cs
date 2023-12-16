using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class VicinityService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveVicinity(Vicinity input)
        {
            DbManager.Vicinity.InsertOneAsync(input).Wait();
            return true;
        }

        public Vicinity GetVicinity(string id)
        {
            var filter = Builders<Vicinity>.Filter.Eq("ID", ObjectId.Parse(id));
            var vicinity =
                DbManager.Vicinity.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return vicinity;
        }
    }
}