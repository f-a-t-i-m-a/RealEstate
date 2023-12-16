using Compositional.Composer;
using JahanJooy.Common.Util.Cache;
using JahanJooy.RealEstate.Core.Components.Dto;

namespace JahanJooy.RealEstate.Core.Cache
{
    [Contract]
    public interface IUserBillingBalanceCache : IItemCache<long, UserBillingBalance>
    {
    }
}