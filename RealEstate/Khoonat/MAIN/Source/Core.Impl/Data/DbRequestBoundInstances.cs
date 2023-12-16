using System;
using System.Collections.Generic;
using System.Data.Entity;
using Compositional.Composer;
using Compositional.Composer.Web.Cache;
using Elmah;

namespace JahanJooy.RealEstate.Core.Impl.Data
{
	[Contract]
	[Component]
	[ComponentCache(typeof(RequestComponentCache))]
	public class DbRequestBoundInstances : DbInstances
	{
		public static bool CanBeUsed
		{
			get { return RequestComponentCache.CanBeUsed; }
		}
	}
}