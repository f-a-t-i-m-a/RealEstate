using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Index;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Impl.Index.Base
{
	public abstract class ObjectIndexBase<TObject> : IObjectIndex<TObject> where TObject : class
	{
		#region Component plugs

		[ComponentPlug]
		public LuceneIndexManager IndexManager { get; set; }

		[ComponentPlug]
		public IComposer Composer { get; set; }

		#endregion

		#region IEntityIndex implementation

		public virtual void AddOrReplace(IEnumerable<TObject> objects)
		{
			if (!objects.SafeAny())
				return;

			using (var writerRef = IndexManager.AcquireWriter(IndexID))
			{
				var indexMap = Composer.GetComponent<IObjectIndexMapper<TObject>>();

				foreach (var obj in objects)
				{
					var searchQuery = GetIdentityQuery(obj);
					writerRef.DeleteDocuments(searchQuery);

					indexMap.PopulateDocument(obj);
					writerRef.AddDocument(indexMap.GetDocument());
				}
			}
		}

		public void Delete(IEnumerable<TObject> objects)
		{
			using (var writerRef = IndexManager.AcquireWriter(IndexID))
			{
				foreach (var obj in objects)
				{
					var searchQuery = GetIdentityQuery(obj);
					writerRef.DeleteDocuments(searchQuery);
				}
			}
		}

		#endregion

		#region Abstract members

		public abstract string IndexID { get; }
		protected abstract Query GetIdentityQuery(TObject obj);

		#endregion
	}
}