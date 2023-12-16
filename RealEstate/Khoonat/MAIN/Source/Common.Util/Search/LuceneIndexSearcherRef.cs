using System;
using log4net;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace JahanJooy.Common.Util.Search
{
	public class LuceneIndexSearcherRef : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(LuceneIndexSearcherRef));

		private IndexSearcher _indexSearcher;
		private LuceneIndexWrapper _indexWrapper;
		private bool _disposed;

		internal IndexSearcher IndexSearcher
		{
			get { return _indexSearcher; }
		}

		internal bool Disposed
		{
			get { return _disposed; }
		}

		internal LuceneIndexSearcherRef(IndexSearcher indexSearcher, LuceneIndexWrapper indexWrapper)
		{
			if (indexSearcher == null)
				throw new ArgumentNullException("indexSearcher");
			if (indexWrapper == null)
				throw new ArgumentNullException("indexWrapper");

			_indexSearcher = indexSearcher;
			_indexWrapper = indexWrapper;

			Log.Debug("Object constructed");
		}

		private void EnsureNotDisposed()
		{
			if (_disposed)
			{
				Log.Warn("Access to an already-disposed IndexSearcher attempted.");
				throw new InvalidOperationException("IndexSearcher reference already disposed, and cannot be used anymore.");
			}
		}

		#region IDisposable implementation

		public void Dispose()
		{
			if (_disposed)
				return;

			Log.Debug("Object disposed");
			_indexWrapper.ReleaseSearcher(this);

			_indexSearcher = null;
			_indexWrapper = null;
			_disposed = true;
		}

		#endregion

		#region Searcher and Reader proxy methods

		public TopDocs Search(Query query, int n)
		{
			EnsureNotDisposed();
			Log.Debug("Search(query, n)");

			try
			{
				return _indexSearcher.Search(query, n);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public TopFieldDocs Search(Query query, Filter filter, int n, Sort sort)
		{
			EnsureNotDisposed();
			Log.Debug("Search(query, filter, n, sort)");

			try
			{
				return _indexSearcher.Search(query, filter, n, sort);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public void Search(Query query, Collector results)
		{
			EnsureNotDisposed();
			Log.Debug("Search(query, results)");

			try
			{
				_indexSearcher.Search(query, results);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public void Search(Weight weight, Filter filter, Collector collector)
		{
			EnsureNotDisposed();
			Log.Debug("Search(weight, filter, collector)");

			try
			{
				_indexSearcher.Search(weight, filter, collector);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public TopFieldDocs Search(Weight weight, Filter filter, int nDocs, Sort sort, bool fillFields)
		{
			EnsureNotDisposed();
			Log.Debug("Search(weight, filter, nDocs, sort, fillFields)");

			try
			{
				return _indexSearcher.Search(weight, filter, nDocs, sort, fillFields);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public TopFieldDocs Search(Weight weight, Filter filter, int nDocs, Sort sort)
		{
			EnsureNotDisposed();
			Log.Debug("Search(weight, filter, nDocs, sort)");

			try
			{
				return _indexSearcher.Search(weight, filter, nDocs, sort);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public TopDocs Search(Weight weight, Filter filter, int nDocs)
		{
			EnsureNotDisposed();
			Log.Debug("Search(weight, filter, nDocs)");

			try
			{
				return _indexSearcher.Search(weight, filter, nDocs);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public void Search(Query query, Filter filter, Collector results)
		{
			EnsureNotDisposed();
			Log.Debug("Search(query, filter, results)");

			try
			{
				_indexSearcher.Search(query, filter, results);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public TopDocs Search(Query query, Filter filter, int n)
		{
			EnsureNotDisposed();
			Log.Debug("Search(query, filter, n)");

			try
			{
				return _indexSearcher.Search(query, filter, n);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		public Document Doc(int i)
		{
			EnsureNotDisposed();
			Log.Debug("Doc(i)");

			try
			{
				return _indexSearcher.Doc(i);
			}
			catch (Exception e)
			{
				_indexWrapper.AddSearcherError(e);
				throw;
			}
		}

		#endregion
	}
}