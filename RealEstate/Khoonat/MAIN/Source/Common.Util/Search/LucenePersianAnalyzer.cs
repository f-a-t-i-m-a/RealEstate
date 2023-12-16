using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace JahanJooy.Common.Util.Search
{
	public class LucenePersianAnalyzer : StandardAnalyzer
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(LucenePersianAnalyzer));

		public LucenePersianAnalyzer()
			: base(Version.LUCENE_30)
		{
			// TODO: Customize this to do Persian-specific analysis, stemming, stop-word removal, and normalization
		}
	}
}