using System.Linq;
using Compositional.Composer;
using Compositional.Composer.Cache;
using JahanJooy.Common.Util.Cache;
using JahanJooy.Common.Util.Cache.Components;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Application.Security
{
    [Contract]
    public interface IHttpSessionCache : IItemCache<string, HttpSession>
    {
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class HttpSessionCache : AutoLoadItemCache<string, HttpSession>, IHttpSessionCache
    {
        protected override int DefaultMaintenanceFrequencySeconds => 5*60;
        protected override int DefaultIdleSecondsToRemove => 10*60;
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class HttpSessionCacheItemLoader : ICacheItemLoader<string, HttpSession>
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public HttpSession Load(string key)
        {
            return
                DbManager.Db.HttpSessions.Where(s => s.HttpSessionID == key)
                    .OrderByDescending(s => s.ID)
                    .FirstOrDefault();
        }
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class HttpSessionCacheValueCopier : ICacheValueCopier<HttpSession>
    {
        public HttpSession Copy(HttpSession original)
        {
            return HttpSession.Copy(original);
        }
    }
}