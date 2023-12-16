using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Index
{
    [Contract]
    public interface IAgencyIndex : IEntityIndex<Agency>
    {
        PagedList<long> Search(long? vicinityID, int agencyCount, int? pageNum);
        List<long> SearchByName(string query, int skipCount, int pageSize);
    }
}