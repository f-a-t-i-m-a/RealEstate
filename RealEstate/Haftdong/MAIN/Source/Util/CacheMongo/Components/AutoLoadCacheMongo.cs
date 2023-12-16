using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using Compositional.Composer.Cache;

namespace JahanJooy.RealEstateAgency.Util.CacheMongo.Components
{
	[Component]
	[ComponentCache(typeof(DefaultComponentCache))]
	[IgnoredOnAssemblyRegistration]
	public class AutoLoadCacheMongo<TKey, TValue> : ICacheMongo<TKey, TValue>
	{
		protected ConcurrentDictionary<TKey, TValue> CacheData;
		protected bool CacheDataValid;
		
		#region Plugs and Configuration points

		[ComponentPlug]
		public ICacheLoaderMongo<TValue> Loader { get; set; }

		[ComponentPlug]
		public ICacheKeyMapperMongo<TKey, TValue> KeyMapper { get; set; }

		[ComponentPlug]
		public ICacheValueCopierMongo<TValue> Copier { get; set; } 

		#endregion

		#region Initialization

		public AutoLoadCacheMongo()
		{
			CacheData = null;
			CacheDataValid = false;
		}

		#endregion

		#region Implementation of ICache<in TKey,out TValue>

		public void InvalidateAll()
		{
			CacheDataValid = false;
		}

		public TValue this[TKey key]
		{
			get
			{
				EnsureLoaded();
				return CacheData.ContainsKey(key) ? Copier.Copy(CacheData[key]) : default(TValue);
			}
		}

		#endregion

		#region Private helper methods

		protected void EnsureLoaded()
		{
			if (CacheDataValid && CacheData != null)
				return;

			lock(this)
			{
				if (CacheDataValid && CacheData != null)
					return;

				Reload();
			}
		}

		private void Reload()
		{
			var all = Loader.LoadAll();
			IndexLoadedData(all);

			CacheDataValid = true;
		}

		protected virtual void IndexLoadedData(IEnumerable<TValue> all)
		{
			CacheData = new ConcurrentDictionary<TKey, TValue>(all.ToDictionary(item => KeyMapper.MapKey(item)));
		}

		#endregion
	}
}