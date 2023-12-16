using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Search;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Impl.Index.Base;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Domain.Directory;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Component]
    public class AgencyBranchIndex : EntityIndexBase<AgencyBranch>, IAgencyBranchIndex
	{
		private const string IndexIDString = "AgencyBranch";

		#region Initialization

		static AgencyBranchIndex()
		{
			LuceneIndexManager.RegisterConfigurationKeys(IndexIDString);
		}

		[OnCompositionComplete]
		public void OnCompositionComplete()
		{
			IndexManager.InitializeIndex(IndexIDString);
		}

		#endregion

		#region Overrides

		public override string IndexID
		{
			get { return IndexIDString; }
		}

		protected override string IdentityFieldName
		{
			get { return AgencyBranchIndexMap.FieldNames.ID; }
		}

		protected override long GetEntityID(AgencyBranch entity)
		{
			return entity.ID;
		}

		#endregion

		public IEnumerable<long> SearchAgencyBranches(LatLngBounds bounds)
        {
            const int startIndex = 0;
            const int pageSize = 20;
            var luceneQuery = new BooleanQuery();
            if (bounds != null)
            {
                var latQuery = NumericRangeQuery.NewDoubleRange(AgencyBranchIndexMap.FieldNames.GeographicLocationLat,
                    bounds.SouthLat,
                    bounds.NorthLat, true, true);
                luceneQuery.Add(latQuery, Occur.MUST);
                var lngQuery = NumericRangeQuery.NewDoubleRange(AgencyBranchIndexMap.FieldNames.GeographicLocationLng,
                    bounds.WestLng,
                    bounds.EastLng, true, true);
                luceneQuery.Add(lngQuery, Occur.MUST);
            }
            var result = new List<long>();
            using (var searcherRef = IndexManager.AcquireSearcher(IndexIDString))
            {
                TopDocs topDocs = searcherRef.Search(luceneQuery, startIndex + pageSize);
                foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
                {
                    var doc = searcherRef.Doc(scoreDoc.Doc);
                    var agencyBranchID = long.Parse(doc.Get(AgencyBranchIndexMap.FieldNames.ID));
                    result.Add(agencyBranchID);
                }
			}
            return result;
		}
	}
}