using System;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.Search
{
	public class LuceneIndexStatistics : LuceneIndexHealthStatus
	{
		public long TotalSizeBytes { get; set; }
		public long TotalNumberOfDocuments { get; set; }
		public Dictionary<string, long> FileSizes { get; set; }

		public DateTime? InitializationTimeUtc { get; set; }
		public DateTime? LastReopenTimeUtc { get; set; }
		public long TimesSearcherAcquiredAfterInitialization { get; set; }
		public long TimesWriterAcquiredAfterInitialization { get; set; }
		public long TimesCommittedAfterInitialization { get; set; }
		public long TimesOptimizedAfterInitialization { get; set; }
		public long TimesReopenedAfterInitialization { get; set; }

		public int CurrentSearcherRefCount { get; set; }
		public int CurrentWriterRefCount { get; set; }
		public int CurrentOutstandingChanges { get; set; }
	}
}