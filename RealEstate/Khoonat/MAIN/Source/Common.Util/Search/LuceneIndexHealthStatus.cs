using System;

namespace JahanJooy.Common.Util.Search
{
	public class LuceneIndexHealthStatus
	{
		public string IndexID { get; set; }
		public bool Initialized { get; set; }
		public bool ShutDown { get; set; }

		public int NumberOfErrors { get; set; }
		public DateTime? IndexSearcherLastUsedTimeUtc { get; set; }
		public DateTime? IndexWriterLastUsedTimeUtc { get; set; }
		public DateTime? LastCommitTimeUtc { get; set; }
		public DateTime? LastOptimizationTimeUtc { get; set; }

		public bool HasErrors
		{
			get { return NumberOfErrors > 0; }
		}
	}
}