
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Owin;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    public static class OwinRequestScopeContextExtensions
    {
        private const string EnvironmentKey = "jj.RealEstateAgency.UserActivity";

        public static UserActivityOwinEntry GetUserActivityEntry(this OwinRequestScopeContext context)
        {
            return context.IfNotNull(c => c.OwinContext.Get<UserActivityOwinEntry>(EnvironmentKey));
        }


        public static UserActivityOwinEntry EnsureUserActivityEntry(this OwinRequestScopeContext context)
        {
            var entry = context.OwinContext.Get<UserActivityOwinEntry>(EnvironmentKey);
            if (entry == null)
            {
                entry = new UserActivityOwinEntry();
                context.OwinContext.Set(EnvironmentKey, entry);
            }

            return entry;
        }
    }
}