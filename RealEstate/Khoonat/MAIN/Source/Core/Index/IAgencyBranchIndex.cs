using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Index
{
    [Contract]
    public interface IAgencyBranchIndex : IEntityIndex<AgencyBranch>
    {
        IEnumerable<long> SearchAgencyBranches(LatLngBounds bounds);
    }
}