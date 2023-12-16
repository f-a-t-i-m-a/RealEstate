using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Cache;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Cache
{
    [Contract]
    public interface IVicinityCache : ICache<long, Vicinity>
    {
		Vicinity Root { get; }
	    List<Vicinity> Search(string query, bool canContainPropertyRecordsOnly, int index = 0, int pageSize = 20, long? rootVicinityId = null, bool includeDisabled = false);
    }
}
