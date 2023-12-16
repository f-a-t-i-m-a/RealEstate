using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.CacheMongo;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.VicinityCache
{
    [Contract]
    public interface IVicinityCache : ICacheMongo<ObjectId, Vicinity>
    {
        Vicinity Root { get; }
        List<Vicinity> Search(string query, bool canContainPropertyRecordsOnly, int index = 0, int pageSize = 20, ObjectId? rootVicinityId = null, bool includeDisabled = false);

        IEnumerable<Vicinity> GetParents(ObjectId vicinityId);

        IEnumerable<Vicinity> GetParentsInclusive(ObjectId vicinityId);
    }
}