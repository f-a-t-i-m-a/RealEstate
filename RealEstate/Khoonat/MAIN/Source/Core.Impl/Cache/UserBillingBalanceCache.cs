using Compositional.Composer;
using JahanJooy.Common.Util.Cache;
using JahanJooy.Common.Util.Cache.Components;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;

namespace JahanJooy.RealEstate.Core.Impl.Cache
{
    [Component]
    public class UserBillingBalanceCache : AutoLoadItemCache<long, UserBillingBalance>, IUserBillingBalanceCache
    {
        protected override int DefaultMaximumLifetimeSeconds { get { return 60*60; } }
        protected override int DefaultMaintenanceFrequencySeconds { get { return 2*60*60; } }
    }

    [Component]
    public class UserBillingBalanceCacheItemLoader : ICacheItemLoader<long, UserBillingBalance>
    {
        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public UserBillingBalance Load(long key)
        {
            using (DbManager.PushNewThreadBoundScope())
            {
                return UserBillingComponent.CalculateBalance(key);
            }
        }
    }

    [Component]
    public class UserBillingBalanceCacheValueCopier : ICacheValueCopier<UserBillingBalance>
    {
        public UserBillingBalance Copy(UserBillingBalance original)
        {
            // For now, don't copy the values.
            // We should be careful not to modify values retrieved from cache.
            
            // TODO: Reconsider this

            return original;
        }
    }
}