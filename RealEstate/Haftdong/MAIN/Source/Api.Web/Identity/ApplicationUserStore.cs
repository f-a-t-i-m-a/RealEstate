using AspNet.Identity.MongoDB;
using Compositional.Composer;
using Compositional.Composer.Web.Cache;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.Web.Identity
{
    [Contract]
    [Component]
    [ComponentCache(typeof(OwinContextComponentCache))]
    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore(IMongoCollection<ApplicationUser> users) : base(users)
        {
        }

        [CompositionConstructor]
        public ApplicationUserStore(DbManager dbManager) : base(dbManager.ApplicationUser)
        {
        }
    }
}