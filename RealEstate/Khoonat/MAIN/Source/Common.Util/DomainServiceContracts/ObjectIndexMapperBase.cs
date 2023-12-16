using System;
using Lucene.Net.Documents;

namespace JahanJooy.Common.Util.DomainServiceContracts
{
	public abstract class ObjectIndexMapperBase<T> : IObjectIndexMapper<T>
	{
		protected readonly Document Document;

		protected ObjectIndexMapperBase()
		{
			DocumentReady = false;
			Document = new Document();
		}

		public void PopulateDocument(T obj)
		{
			if (DocumentReady)
			{
				// TODO: Log warning
			}

			SetFieldValues(obj);
			DocumentReady = true;
		}

		public Document GetDocument()
		{
			if (!DocumentReady)
				throw new InvalidOperationException("Document is not populated, or is consumed before. Possibly a development issue.");

			DocumentReady = false;
			return Document;
		}

		public bool DocumentReady { get; private set; }

		protected abstract void SetFieldValues(T entity);
	}
}