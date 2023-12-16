using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Impl.Index.Base;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Domain.Directory;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
    [Component]
    public class AgencyIndex : EntityIndexBase<Agency>, IAgencyIndex
    {
        private const string IndexIDString = "Agency";

        #region Initialization

        static AgencyIndex()
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
            get { return AgencyIndexMap.FieldNames.ID; }
        }

        protected override long GetEntityID(Agency entity)
        {
            return entity.ID;
        }

        #endregion

        public PagedList<long> Search(long? vicinityID, int agencyCount, int? pageNum)
        {
            const int pageSize = 20;

            if (pageNum == null || pageNum < 1)
                pageNum = 1;

            var startIndex = (int) ((pageNum - 1)*pageSize);

            var luceneQuery = new BooleanQuery();
            if (vicinityID.HasValue)
            {
                var vicinityQuery =
                    new TermQuery(new Term(AgencyIndexMap.FieldNames.VicinityIds,
                        vicinityID.Value.ToString(CultureInfo.InvariantCulture)));
                luceneQuery.Add(vicinityQuery, Occur.MUST);
            }

            var notDeletedQuery = new TermQuery(new Term(AgencyIndexMap.FieldNames.Deleted, false.ToString()));
            luceneQuery.Add(notDeletedQuery, Occur.MUST);

            if (!luceneQuery.Clauses.Any())
                luceneQuery.Add(new MatchAllDocsQuery(), Occur.SHOULD);

            using (var searcherRef = IndexManager.AcquireSearcher(IndexIDString))
            {
                TopDocs topDocs = searcherRef.Search(luceneQuery, startIndex + pageSize);
                var result = PagedList<long>.BuildUsingStartIndex(topDocs.TotalHits, pageSize, startIndex);
                result.PageItems = new List<long>();

                foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
                {
                    var doc = searcherRef.Doc(scoreDoc.Doc);
                    var agencyID = long.Parse(doc.Get(AgencyIndexMap.FieldNames.ID));
                    result.PageItems.Add(agencyID);
                }
                return result;
            }
        }

        public List<long> SearchByName(string query, int startIndex = 0, int pageSize = 20)
        {
            var luceneQuery = new BooleanQuery();

            var nameQuery = new BooleanQuery();
            luceneQuery.Add(nameQuery, Occur.MUST);
            foreach (var inputTerm in UserSearchQueryUtils.TokenizeUserQuery(query))
            {
                if (string.IsNullOrWhiteSpace(inputTerm))
                    continue;

                var inputTermQuery = new PrefixQuery(new Term(AgencyIndexMap.FieldNames.Name, query));

                nameQuery.Add(inputTermQuery, Occur.MUST);
            }

            var result = new List<long>();

            using (var searcherRef = IndexManager.AcquireSearcher(IndexIDString))
            {
                TopDocs topDocs = searcherRef.Search(luceneQuery, startIndex + pageSize);
                if (topDocs.ScoreDocs.Length < startIndex)
                    return result;

                foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
                {
                    var doc = searcherRef.Doc(scoreDoc.Doc);
                    var agencyID = long.Parse(doc.Get(AgencyIndexMap.FieldNames.ID));
                    result.Add(agencyID);
                }
                return result;
            }
        }
    }
}