using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using Lucene.Net.Search;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Index.Base
{
	public abstract class EntityIndexBase<TEntity> : ObjectIndexBase<TEntity>, IEntityIndex<TEntity> where TEntity : class, IIndexedEntity
	{
		#region Component plugs

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		#endregion

		#region Abstract members

		protected abstract string IdentityFieldName { get; }
		protected abstract long GetEntityID(TEntity entity);

		#endregion

		#region IEntityIndex implementation

		public Type EntityType
		{
			get { return typeof (TEntity); }
		}

		public override void AddOrReplace(IEnumerable<TEntity> entities)
		{
			base.AddOrReplace(entities);

			var csIds = entities.Select(GetEntityID).Join(",");
			DbManager.Db.Database.ExecuteSqlCommand("UPDATE [{0}] SET [IndexedTime] = GETDATE() WHERE ID IN ({1})".Fmt(TableName, csIds));
		}

		#endregion

		protected virtual string TableName
		{
			get { return typeof (TEntity).Name; }
		}

		protected override Query GetIdentityQuery(TEntity entity)
		{
			return NumericRangeQuery.NewLongRange(IdentityFieldName, GetEntityID(entity), GetEntityID(entity), true, true);
		}
	}
}