using System;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace JahanJooy.Common.Util.Search
{
	public class LuceneIndexWriterRef : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(LuceneIndexWriterRef));

		private IndexWriter _indexWriter;
		private LuceneIndexWrapper _indexWrapper;
		private bool _disposed;
		private bool _dirty;

		internal IndexWriter IndexWriter
		{
			get { return _indexWriter; }
		}

		internal bool Disposed
		{
			get { return _disposed; }
		}

		internal bool Dirty
		{
			get { return _dirty; }
		}

		internal LuceneIndexWriterRef(IndexWriter indexWriter, LuceneIndexWrapper indexWrapper)
		{
			if (indexWriter == null)
				throw new ArgumentNullException("indexWriter");
			if (indexWrapper == null)
				throw new ArgumentNullException("indexWrapper");

			_indexWriter = indexWriter;
			_indexWrapper = indexWrapper;

			Log.Debug("Object constructed");
		}

		private void EnsureNotDisposed()
		{
			if (_disposed)
			{
				Log.Warn("Access to an already-disposed IndexWriter attempted.");
				throw new InvalidOperationException("IndexWriter reference already disposed, and cannot be used anymore.");
			}
		}

		#region IDisposable implementation

		public void Dispose()
		{
			if (_disposed)
				return;

			Log.Debug("Object disposed");
			_indexWrapper.ReleaseWriter(this);

			_indexWriter = null;
			_indexWrapper = null;
			_disposed = true;
		}

		#endregion

		#region Writer proxy methods

		public void DeleteDocuments(Term term)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("DeleteDocuments(term)");

			try
			{
				_indexWriter.DeleteDocuments(term);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void DeleteDocuments(params Term[] terms)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("DeleteDocuments(terms)");

			try
			{
				_indexWriter.DeleteDocuments(terms);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void DeleteDocuments(Query query)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("DeleteDocuments(query)");

			try
			{
				_indexWriter.DeleteDocuments(query);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void DeleteDocuments(params Query[] queries)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("DeleteDocuments(queries)");

			try
			{
				_indexWriter.DeleteDocuments(queries);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void UpdateDocument(Term term, Document doc)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("UpdateDocument(term, doc)");

			try
			{
				_indexWriter.UpdateDocument(term, doc);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void UpdateDocument(Term term, Document doc, Analyzer analyzer)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("UpdateDocument(term, doc, analyzer)");

			try
			{
				_indexWriter.UpdateDocument(term, doc, analyzer);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void AddDocument(Document doc)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("AddDocument(doc)");

			try
			{
				_indexWriter.AddDocument(doc);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		public void AddDocument(Document doc, Analyzer analyzer)
		{
			EnsureNotDisposed();
			_dirty = true;
			Log.Debug("AddDocument(doc, analyzer)");

			try
			{
				_indexWriter.AddDocument(doc, analyzer);
			}
			catch (Exception e)
			{
				_indexWrapper.AddWriterError(e);
				throw;
			}
		}

		#endregion
	}
}