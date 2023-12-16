using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class ApplicationUserService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveApplicationUser(ApplicationUser input)
        {
            DbManager.ApplicationUser.InsertOneAsync(input).Wait();
            return true;
        }

        public ApplicationUser GetApplicationUser(string id)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var user =
                DbManager.ApplicationUser.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return user;
        }
    }
}