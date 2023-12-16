using System.Web;
using Compositional.Composer.Cache;

namespace Compositional.Composer.Web.Cache
{
	[Contract]
	[Component]
	[ComponentCache(typeof(StaticComponentCache))]
	public class RequestComponentCache : IComponentCache
	{
		private readonly object _nullRequestSyncObject = new object();

		public static bool CanBeUsed { get { return HttpContext.Current != null; } }

		#region Implementation of IComponentCache

		public ComponentCacheEntry GetFromCache(ContractIdentity contract)
		{
			if (HttpContext.Current == null)
				return null;

			if (HttpContext.Current.Items.Contains(contract))
				return HttpContext.Current.Items[contract] as ComponentCacheEntry;

			return null;
		}

		public void PutInCache(ContractIdentity contract, ComponentCacheEntry entry)
		{
			HttpContext.Current.Items[contract] = entry;
		}

		public object SynchronizationObject
		{
			get { return HttpContext.Current ?? _nullRequestSyncObject; }
		}

		#endregion
	}
}