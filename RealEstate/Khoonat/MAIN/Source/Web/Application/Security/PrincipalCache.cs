using Compositional.Composer;
using Compositional.Composer.Cache;
using JahanJooy.Common.Util.Cache;
using JahanJooy.Common.Util.Cache.Components;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services;

namespace JahanJooy.RealEstate.Web.Application.Security
{
	[Contract]
	public interface IPrincipalCache : IItemCache<long, CorePrincipal>
	{
	}

	[Component]
	[ComponentCache(typeof(DefaultComponentCache))]
	public class PrincipalCache : AutoLoadItemCache<long, CorePrincipal>, IPrincipalCache
	{
		protected override int DefaultMaximumLifetimeSeconds => 15*60;
	    protected override int DefaultMaintenanceFrequencySeconds => 0;
	}

	[Component]
	[ComponentCache(typeof(DefaultComponentCache))]
	public class PrincipalCacheItemLoader : ICacheItemLoader<long, CorePrincipal>
	{
		[ComponentPlug]
		public IAuthenticationService AuthenticationService { get; set; }

		public CorePrincipal Load(long key)
		{
			return AuthenticationService.LoadPrincipal(key);
		}
	}

	[Component]
	[ComponentCache(typeof(DefaultComponentCache))]
	public class PrincipalCacheValueCopier : ICacheValueCopier<CorePrincipal>
	{
		public CorePrincipal Copy(CorePrincipal original)
		{
			return CorePrincipal.Copy(original);
		}
	}
}