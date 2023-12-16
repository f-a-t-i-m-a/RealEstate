using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using log4net;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using ServiceStack;

namespace JahanJooy.Common.Util.Search
{
	internal class LuceneIndexWrapper
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(LuceneIndexWrapper));

		private const string SettingKeyForLocationFormat = "LuceneIndex.{0}.Location";
		private const int MaxErrorsToKeep = 100;

		private readonly string _id;
		private readonly string _settingKeyForLocation;

		private readonly object _searcherSync = new object();
		private readonly object _searcherReopenSync = new object();

		private volatile bool _initialized;
		private bool _shutDown;
		private int _writerRefCount;
		private int _searcherRefCount;
		private int _closePendingSearcherRefCount;
		private int _outstandingChanges;

		private long _firstOutstandingChangeTime;
		private long _lastReopenTime;

		private Directory _indexDirectory;
		private IndexWriter _indexWriter;
		private IndexSearcher _indexSearcher;
		private IndexReader _indexReader;

		private IndexSearcher _closePendingSearcher;
		private IndexReader _closePendingReader;

		private long _initializationTime;
		private long _indexSearcherLastUsedTime;
		private long _indexWriterLastUsedTime;
		private long _lastCommitTime;
		private long _lastOptimizationTime;

		private int _timesSearcherAcquired;
		private int _timesWriterAcquired;
		private int _timesCommitted;
		private int _timesOptimized;
		private int _timesReopened;

		private readonly ConcurrentQueue<Exception> _writerErrors;
		private readonly ConcurrentQueue<Exception> _searcherErrors;

		private static readonly long ConsecutiveReopenDelayTicks = ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Development
			? TimeSpan.FromSeconds(1).Ticks
			: TimeSpan.FromMinutes(2).Ticks;

		private static readonly long MinimumConsecutiveReopenDelayTicks = TimeSpan.FromSeconds(1).Ticks;

		private static readonly long ReopenDelayAfterChangeTicks = ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Development
			? TimeSpan.FromSeconds(1).Ticks
			: TimeSpan.FromMinutes(1).Ticks;

		[ComponentPlug]
		public IApplicationSettings ApplicationSettings { get; set; }

		#region Initialization

		public static void RegisterConfigurationKeys(string indexId)
		{
			ApplicationSettingKeys.RegisterKey(SettingKeyForLocationFormat.Fmt(indexId));
		}

		public LuceneIndexWrapper(string id)
		{
			if (id.IsNullOrEmpty())
				throw new ArgumentNullException("id");

			_id = id;
			_settingKeyForLocation = SettingKeyForLocationFormat.Fmt(id);
			_writerErrors = new ConcurrentQueue<Exception>();
			_searcherErrors = new ConcurrentQueue<Exception>();
		}

		public void DeleteFilesAndRebuild()
		{
			lock (_searcherReopenSync)
			{
				lock (_searcherSync)
				{
					if (_shutDown)
						throw new InvalidOperationException("The IndexWrapper is already shut down, and cannot be used any more.");

					if (_initialized)
						throw new InvalidOperationException("The IndexWrapper is already initialized. Re-creating index files should be performed before initialization.");

					Log.WarnFormat("IDX[{0}] Deleting all files and rebuilding index", _id);
					// TODO
				}
			}
		}

		#endregion

		#region Internal services

		internal LuceneIndexWriterRef AcquireWriter()
		{
			EnsureInitialized();

			// No need to synchronize, because _indexWriter reference never changes after being initialized
			var writerRefCount = Interlocked.Increment(ref _writerRefCount);
			Log.DebugFormat("IDX[{0}] AcquireWriter -> Incremented _writerRefCount to {1}", _id, writerRefCount);
			Interlocked.Exchange(ref _indexWriterLastUsedTime, DateTime.UtcNow.Ticks);
			Interlocked.Increment(ref _timesWriterAcquired);

			return new LuceneIndexWriterRef(_indexWriter, this);
		}

		internal LuceneIndexSearcherRef AcquireSearcher()
		{
			EnsureInitialized();
			CheckForReopen();

			IndexSearcher indexSearcher;
			int searcherRefCount;

			lock (_searcherSync)
			{
				searcherRefCount = ++_searcherRefCount; // No need to Interlock, all accesses are synchronized.
				indexSearcher = _indexSearcher;
			}

			Log.DebugFormat("IDX[{0}] AcquireSearcher -> Incremented _searcherRefCount to {1}", _id, searcherRefCount);
			Interlocked.Exchange(ref _indexSearcherLastUsedTime, DateTime.UtcNow.Ticks);
			Interlocked.Increment(ref _timesSearcherAcquired);

			// Create the result object outside of "lock" to minimize locking time
			return new LuceneIndexSearcherRef(indexSearcher, this);
		}

		internal void ReleaseWriter(LuceneIndexWriterRef writer)
		{
			var writerRefCount = Interlocked.Decrement(ref _writerRefCount);
			Log.DebugFormat("IDX[{0}] ReleaseWriter -> Decremented _writerRefCount to {1}", _id, writerRefCount);

			if (writerRefCount < 0)
				Log.WarnFormat("IDX[{0}] Reached negative _writerRefCount: {1}", _id, writerRefCount);

			if (writer.Dirty)
				IncrementOutstandingChanges();
		}

		internal void ReleaseSearcher(LuceneIndexSearcherRef searcher)
		{
			bool needToCallCheckClosePendingSearcher = false;

			bool mainSearcherMatched = false;
			bool closePendingSearcherMatched = false;

			int searcherRefCount = 0;
			int closePendingSearcherRefCount = 0;

			lock (_searcherSync)
			{
				if (ReferenceEquals(searcher.IndexSearcher, _indexSearcher))
				{
					mainSearcherMatched = true;
					searcherRefCount = --_searcherRefCount; // No need to Interlock, all accesses are synchronized
				}
				else if (ReferenceEquals(searcher.IndexSearcher, _closePendingSearcher))
				{
					closePendingSearcherMatched = true;
					closePendingSearcherRefCount = Interlocked.Decrement(ref _closePendingSearcherRefCount);
					needToCallCheckClosePendingSearcher = _closePendingSearcherRefCount <= 0;
				}
				// else, log a warning after the lock is released
			}

			if (mainSearcherMatched)
			{
				Log.DebugFormat("IDX[{0}] ReleaseSearcher -> Decremented _searcherRefCount to {1}", _id, searcherRefCount);
				if (searcherRefCount < 0)
					Log.WarnFormat("IDX[{0}] Reached negative _searcherRefCount: {1}", _id, searcherRefCount);
			}

			if (closePendingSearcherMatched)
			{
				Log.DebugFormat("IDX[{0}] ReleaseSearcher -> Decremented _closePendingSearcherRefCount to {1}", _id, closePendingSearcherRefCount);
				if (closePendingSearcherRefCount < 0)
					Log.WarnFormat("IDX[{0}] Reached negative _closePendingSearcherRefCount: {1}", _id, closePendingSearcherRefCount);

			}

			if (!mainSearcherMatched && !closePendingSearcherMatched)
				Log.WarnFormat("IDX[{0}] ReleaseSearcher called with an unrecognized LuceneIndexSearcherRef, possibly lost reference", _id);


			if (needToCallCheckClosePendingSearcher)
			{
				Log.DebugFormat("IDX[{0}] Attempting to close the pending searcher, since its ref count reached zero.", _id);
				CheckClosePendingSearcher();
			}
		}

		internal void DeleteAllDocuments()
		{
			EnsureInitialized();

			Log.WarnFormat("IDX[{0}] Deleting all documents", _id);
			Interlocked.Exchange(ref _indexWriterLastUsedTime, DateTime.UtcNow.Ticks);

			try
			{
				_indexWriter.DeleteAll();
			}
			catch (Exception e)
			{
				AddWriterError(e);
				throw;
			}
		}

		internal void Commit()
		{
			EnsureInitialized();

			Log.DebugFormat("IDX[{0}] Commiting changes", _id);
			Interlocked.Exchange(ref _lastCommitTime, DateTime.UtcNow.Ticks);
			Interlocked.Increment(ref _timesCommitted);

			try
			{
				_indexWriter.Commit();
			}
			catch (Exception e)
			{
				AddWriterError(e);
				throw;
			}

			// Make sure the reader re-opens after commit
			// (Probably it's not needed, as the Lucene documentation says that re-open can "see" the
			//  uncommitted changes made to the index)
			IncrementOutstandingChanges();
		}

		internal void Rollback()
		{
			EnsureInitialized();

			Log.DebugFormat("IDX[{0}] Rolling back changes", _id);

			try
			{
				_indexWriter.Rollback();
			}
			catch (Exception e)
			{
				AddWriterError(e);
				throw;
			}

			IncrementOutstandingChanges();
		}

		internal void Optimize()
		{
			EnsureInitialized();

			Log.DebugFormat("IDX[{0}] Optimizing", _id);
			Interlocked.Exchange(ref _lastOptimizationTime, DateTime.UtcNow.Ticks);
			Interlocked.Increment(ref _timesOptimized);

			try
			{
				_indexWriter.Optimize();
			}
			catch (Exception e)
			{
				AddWriterError(e);
				throw;
			}

			// Make sure the reader re-opens after optimization
			IncrementOutstandingChanges();
		}

		internal void Shutdown()
		{
			_shutDown = true;

			Log.InfoFormat("IDX[{0}] Shutting down", _id);

			// TODO
		}

		internal void AddWriterError(Exception e)
		{
			_writerErrors.Enqueue(e);

			Exception tempException;
			if (_writerErrors.Count > MaxErrorsToKeep)
				_writerErrors.TryDequeue(out tempException);
		}

		internal void AddSearcherError(Exception e)
		{
			_searcherErrors.Enqueue(e);

			Exception tempException;
			if (_searcherErrors.Count > MaxErrorsToKeep)
				_searcherErrors.TryDequeue(out tempException);
		}

		internal void ClearErrors()
		{
			Exception tempException;

			int count = _searcherErrors.Count;
			if (count > 0)
				for (int i = 0; i < count; i++)
					_searcherErrors.TryDequeue(out tempException);

			count = _writerErrors.Count;
			if (count > 0)
				for (int i = 0; i < count; i++)
					_writerErrors.TryDequeue(out tempException);
		}

		internal LuceneIndexHealthStatus GetHealthStatus()
		{
			try
			{
				EnsureInitialized();
			}
			catch (Exception e)
			{
				Log.WarnFormat("IDX[{0}] Error during index initialization: {1}", _id, e.Message);
			}

			var result = new LuceneIndexHealthStatus();
			FillHealthStatus(result);
			return result;
		}

		internal LuceneIndexStatistics GetStatistics()
		{
			try
			{
				EnsureInitialized();
			}
			catch (Exception e)
			{
				Log.WarnFormat("IDX[{0}] Error during index initialization: {1}", _id, e.Message);
			}

			var result = new LuceneIndexStatistics();
			FillHealthStatus(result);
			FillStatistics(result);
			return result;
		}

		#endregion

		#region Internal properties

		public DateTime? IndexSearcherLastUsedTimeUtc
		{
			get
			{
				var indexSearcherLastUsedTime = Interlocked.Read(ref _indexSearcherLastUsedTime);
				return indexSearcherLastUsedTime <= 0 ? (DateTime?) null : new DateTime(indexSearcherLastUsedTime, DateTimeKind.Utc);
			}
		}

		public DateTime? IndexWriterLastUsedTimeUtc
		{
			get
			{
				var indexWriterLastUsedTime = Interlocked.Read(ref _indexWriterLastUsedTime);
				return indexWriterLastUsedTime <= 0 ? (DateTime?)null : new DateTime(indexWriterLastUsedTime, DateTimeKind.Utc);
			}
		}

		public DateTime? LastCommitTimeUtc
		{
			get
			{
				var lastCommitTime = Interlocked.Read(ref _lastCommitTime);
				return lastCommitTime <= 0 ? (DateTime?)null : new DateTime(lastCommitTime, DateTimeKind.Utc);
			}
		}

		public DateTime? LastOptimizationTimeUtc
		{
			get
			{
				var lastOptimizationTime = Interlocked.Read(ref _lastOptimizationTime);
				return lastOptimizationTime <= 0 ? (DateTime?)null : new DateTime(lastOptimizationTime, DateTimeKind.Utc);
			}
		}

		public DateTime? InitializationTimeUtc
		{
			get
			{
				var initializationTime = Interlocked.Read(ref _initializationTime);
				return initializationTime <= 0 ? (DateTime?)null : new DateTime(initializationTime, DateTimeKind.Utc);
			}
		}

		public DateTime? LastReopenTimeUtc
		{
			get
			{
				var lastReopenTime = Interlocked.Read(ref _lastReopenTime);
				return lastReopenTime <= 0 ? (DateTime?)null : new DateTime(lastReopenTime, DateTimeKind.Utc);
			}
		}

		public bool HasErrors
		{
			get { return !_searcherErrors.IsEmpty || !_writerErrors.IsEmpty; }
		}

		public List<Exception> WriterErrors
		{
			get { return _writerErrors.ToList(); }
		}

		public List<Exception> SearcherErrors
		{
			get { return _searcherErrors.ToList(); }
		}

		public List<Exception> AllErrors
		{
			get { return _searcherErrors.Concat(_writerErrors).ToList(); }
		}

		#endregion

		#region Private helper methods

		private void EnsureInitialized()
		{
			if (_shutDown)
				throw new InvalidOperationException("The IndexWrapper is already shut down, and cannot be used any more.");

			if (_initialized)
				return;

			lock (_searcherReopenSync)
			{
				lock (_searcherSync)
				{
					Thread.MemoryBarrier();
					if (_initialized)
						return;

					Log.InfoFormat("IDX[{0}] Initializing", _id);

					var indexLocation = ApplicationSettings[_settingKeyForLocation];
					Log.DebugFormat("IDX[{0}] Openning index from location {1}", _id, indexLocation);
					_indexDirectory = FSDirectory.Open(indexLocation);

					// TODO: Determine if having the following lines here is a good idea or not.
					// TODO: May be we need an index maintenance command from Admin to forcibly unlock.

					if (IndexWriter.IsLocked(_indexDirectory))
					{
						Log.WarnFormat("IDX[{0}] Is locked when openning; forcibly unlocking the directory", _id);
						IndexWriter.Unlock(_indexDirectory);
					}

					try
					{
						_indexWriter = new IndexWriter(_indexDirectory, new LucenePersianAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED);
					}
					catch (Exception e)
					{
						AddWriterError(e);
						throw;
					}

					try
					{
						_indexReader = _indexWriter.GetReader();
						_indexSearcher = new IndexSearcher(_indexReader);
					}
					catch (Exception e)
					{
						AddSearcherError(e);
						throw;
					}

					_searcherRefCount = 0;
					_writerRefCount = 0;
					Interlocked.Exchange(ref _lastReopenTime, DateTime.UtcNow.Ticks);

					_closePendingReader = null;
					_closePendingSearcher = null;
					_closePendingSearcherRefCount = 0;

					_initialized = true;
					_initializationTime = DateTime.UtcNow.Ticks;
				}
			}

			Log.DebugFormat("IDX[{0}] Initialization completed.", _id);
		}

		private void CheckForReopen()
		{
			var outstandingChanges = _outstandingChanges;

			if (outstandingChanges == 0)
				return;

			Log.DebugFormat("IDX[{0}] Outstanding changes: {1}", _id, outstandingChanges);

			var now = DateTime.UtcNow.Ticks;

			// Don't need to re-open in less than the time specified by ReopenDelay
			var lastReopenTime = Interlocked.Read(ref _lastReopenTime);
			if (lastReopenTime + ConsecutiveReopenDelayTicks > now)
			{
				Log.DebugFormat("IDX[{0}] Not re-openning because last re-open time is {1}, which is less than 10 minutes before now ({2}).", _id, lastReopenTime, now);
				return;
			}

			// Don't re-open if the first change is less than ReopenDelayAfterChange ago
			var firstOutstandingChangeTime = Interlocked.Read(ref _firstOutstandingChangeTime);
			if (firstOutstandingChangeTime + ReopenDelayAfterChangeTicks > now)
			{
				Log.DebugFormat("IDX[{0}] Not re-openning because first outstanding change time is {1}, which is less than one minutes before now ({2}).", _id, firstOutstandingChangeTime, now);
				return;
			}

			Reopen();
		}

		private void Reopen()
		{
			Log.DebugFormat("IDX[{0}] Starting to re-open", _id);

			lock (_searcherSync)
			{
				Thread.MemoryBarrier();

				// Never re-open in less than a seconds
				var lastReopenTime = Interlocked.Read(ref _lastReopenTime);
				var now = DateTime.UtcNow.Ticks;

				if (lastReopenTime + MinimumConsecutiveReopenDelayTicks > now)
				{
					Log.DebugFormat("IDX[{0}] Ignoring re-open because last re-open time is {1}, which is less than 10 seconds before now ({2}).", _id, lastReopenTime, now);
					return;
				}

				// Set _lastReopenTime in the beginning to prevent other threads to stack up on the lock while re-openning.
				Interlocked.Exchange(ref _lastReopenTime, now);
				Log.DebugFormat("IDX[{0}] Last re-open time is now {1}.", _id, now);
			}

			lock (_searcherReopenSync)
			{
				IndexReader newReader;
				_timesReopened++;

				try
				{
					newReader = _indexReader.Reopen();
				}
				catch (Exception e)
				{
					AddSearcherError(e);
					throw;
				}

				if (ReferenceEquals(newReader, _indexReader))
				{
					Log.DebugFormat("IDX[{0}] Re-open returned the same reader, which means no changes has been made to the index.", _id);

					// No need to change anything, there is no change in the index store
					return;
				}

				var newSearcher = new IndexSearcher(newReader);

				lock (_searcherSync)
				{
					CleanupClosePendingReader();

					_closePendingReader = _indexReader;
					_closePendingSearcher = _indexSearcher;
					_closePendingSearcherRefCount = _searcherRefCount;

					_indexReader = newReader;
					_indexSearcher = newSearcher;
					_searcherRefCount = 0;

					_outstandingChanges = 0;
					Interlocked.Exchange(ref _firstOutstandingChangeTime, 0);
				}
			}

			Log.DebugFormat("IDX[{0}] Re-open completed without errors", _id);

			CheckClosePendingSearcher();
		}

		private void CleanupClosePendingReader()
		{
			// This method is called inside a "lock", so there's no need for synchronization inside it.

			if (_closePendingReader == null && _closePendingSearcher == null)
				return;

			if (_closePendingSearcher != null)
			{
				Log.WarnFormat("IDX[{0}] The index searcher that was put aside in the previous re-open is not yet disposed. Forcibly disposing it.", _id);

				try
				{
					_closePendingSearcher.Dispose();
				}
				catch (Exception e)
				{
					AddSearcherError(e);
					Log.Error("IDX[" + _id + "] Error encountered during forceful disposal of _closePendingSearcher", e);
				}

				_closePendingSearcher = null;
			}

			if (_closePendingReader != null)
			{
				Log.WarnFormat("IDX[{0}] The index reader that was put aside in the previous re-open is not yet disposed. Forcibly disposing it.", _id);

				try
				{
					_closePendingReader.Dispose();
				}
				catch (Exception e)
				{
					AddSearcherError(e);
					Log.Error("IDX[" + _id + "] Error encountered during forceful disposal of _closePendingReader", e);
				}

				_closePendingReader = null;
			}
		}

		private void CheckClosePendingSearcher()
		{
			if (_closePendingSearcherRefCount > 0)
				return;

			IndexSearcher closePendingSearcher;
			IndexReader closePendingReader;

			lock (_searcherSync)
			{
				Thread.MemoryBarrier();
				if (_closePendingSearcherRefCount > 0)
					return;

				if (_closePendingSearcherRefCount < 0)
				{
					Log.WarnFormat("IDX[{0}] _closePendingSearcherRefCount is not supposed to go below zero, but it is {1}.", _id, _closePendingSearcherRefCount);
				}

				closePendingReader = _closePendingReader;
				closePendingSearcher = _closePendingSearcher;

				_closePendingReader = null;
				_closePendingSearcher = null;
				_closePendingSearcherRefCount = 0;
			}

			Log.DebugFormat("IDX[{0}] Close-pending searcher is not used anymore, disposing.", _id);

			if (closePendingSearcher != null)
			{
				try
				{
					closePendingSearcher.Dispose();
				}
				catch (Exception e)
				{
					AddSearcherError(e);
					Log.Error("IDX[" + _id + "] Error encountered while disposing _closePendingSearcher", e);
				}
			}

			if (closePendingReader != null)
			{
				try
				{
					closePendingReader.Dispose();
				}
				catch (Exception e)
				{
					AddSearcherError(e);
					Log.Error("IDX[" + _id + "] Error encountered while disposing _closePendingReader", e);
				}
			}

			Log.DebugFormat("IDX[{0}] Disposal of _closePendingSearcher and _closePendingReader completed.", _id);
		}

		private void IncrementOutstandingChanges()
		{
			var now = DateTime.UtcNow.Ticks;

			Interlocked.CompareExchange(ref _firstOutstandingChangeTime, now, 0);
			var outstandingChanges = Interlocked.Increment(ref _outstandingChanges);

			Log.DebugFormat("IDX[{0}] Increasing number of oustanding changes to {1}, on {2}", _id, outstandingChanges, now);
		}

		private void FillHealthStatus(LuceneIndexHealthStatus healthStatus)
		{
			healthStatus.IndexID = _id;
			healthStatus.Initialized = _initialized;
			healthStatus.ShutDown = _shutDown;

			healthStatus.NumberOfErrors = _writerErrors.Count + _searcherErrors.Count;

			healthStatus.IndexSearcherLastUsedTimeUtc = IndexSearcherLastUsedTimeUtc;
			healthStatus.IndexWriterLastUsedTimeUtc = IndexWriterLastUsedTimeUtc;
			healthStatus.LastCommitTimeUtc = LastCommitTimeUtc;
			healthStatus.LastOptimizationTimeUtc = LastOptimizationTimeUtc;
		}

		private void FillStatistics(LuceneIndexStatistics statistics)
		{
			statistics.InitializationTimeUtc = InitializationTimeUtc;
			statistics.LastReopenTimeUtc = LastReopenTimeUtc;
			statistics.TimesSearcherAcquiredAfterInitialization = _timesSearcherAcquired;
			statistics.TimesWriterAcquiredAfterInitialization = _timesWriterAcquired;
			statistics.TimesCommittedAfterInitialization = _timesCommitted;
			statistics.TimesOptimizedAfterInitialization = _timesOptimized;
			statistics.TimesReopenedAfterInitialization = _timesReopened;

			statistics.CurrentSearcherRefCount = _searcherRefCount;
			statistics.CurrentWriterRefCount = _writerRefCount;
			statistics.CurrentOutstandingChanges = _outstandingChanges;

			bool fileStatisticsExtracted = false;

			if (_initialized && !_shutDown)
			{
				try
				{
					statistics.FileSizes = _indexDirectory.ListAll().ToDictionary(s => s, s => _indexDirectory.FileLength(s));
					statistics.TotalSizeBytes = statistics.FileSizes.Values.Sum();
					statistics.TotalNumberOfDocuments = _indexWriter.MaxDoc();

					fileStatisticsExtracted = true;
				}
				catch (Exception e)
				{
					Log.Error("IDX[" + _id + "] Error while extracting file statistics: {1}", e);
				}
			}

			if (!fileStatisticsExtracted)
			{
				// Make sure the FileSizes is not null in the result
				statistics.FileSizes = new Dictionary<string, long>();
			}
		}

		#endregion
	}
}