using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using Compositional.Composer.Cache;
using JahanJooy.Common.Util.Cache;
using JahanJooy.Common.Util.Cache.Components;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;


namespace JahanJooy.RealEstate.Core.Impl.Cache
{
    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class VicinityCache : AutoLoadCache<long, Vicinity>, IVicinityCache
    {
	    private Vicinity _rootVicinity;
	    private List<Vicinity> _loadedVicinities;

	    private Directory _indexDirectory;
	    private IndexReader _indexReader;
	    private IndexSearcher _indexSearcher;

		#region Additional methods in IVicinityCache

	    public Vicinity Root
	    {
		    get
		    {
			    EnsureLoaded();
			    return _rootVicinity;
		    }
	    }

		public List<Vicinity> Search(string query, bool canContainPropertyRecordsOnly, int startIndex = 0, int pageSize = 20, long? rootVicinityId = null, bool includeDisabled = false)
		{
			EnsureLoaded();

			var rootQuery = new BooleanQuery();

			if (rootVicinityId.HasValue)
			{
				var scopeQuery = new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.ParentsIDs, rootVicinityId.Value.ToString(CultureInfo.InvariantCulture)));
				rootQuery.Add(scopeQuery, Occur.MUST);
			}

			if (!includeDisabled)
			{
				var enabledQuery = new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.Enabled, true.ToString()));
				rootQuery.Add(enabledQuery, Occur.MUST);
			}

			var nameQuery = new BooleanQuery();
			rootQuery.Add(nameQuery, Occur.MUST);

			foreach (var inputTerm in UserSearchQueryUtils.TokenizeUserQuery(query.ToLower()))
			{
				if (string.IsNullOrWhiteSpace(inputTerm))
					continue;

				var inputTermQuery = new BooleanQuery
			                         {
				                         {new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.Name, inputTerm)) {Boost = 1.5f}, Occur.SHOULD},
				                         {new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.AlternativeNames, inputTerm)) {Boost = 1.4f}, Occur.SHOULD},
				                         {new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.AdditionalSearchText, inputTerm)) {Boost = 1.3f}, Occur.SHOULD},
				                         {new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.LocalizedType, inputTerm)) {Boost = 0.5f}, Occur.SHOULD},
				                         {new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.ParentsSearchText, inputTerm)) {Boost = -1f}, Occur.SHOULD}
			                         };

				nameQuery.Add(inputTermQuery, Occur.MUST);
			}

			if (canContainPropertyRecordsOnly)
			{
				rootQuery.Add(new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.CanContainPropertyRecords, true.ToString())), Occur.MUST);
			}

			var result = new List<Vicinity>();

			TopDocs topDocs = _indexSearcher.Search(rootQuery, startIndex + pageSize);
			if (topDocs.ScoreDocs.Length < startIndex)
				return result;

			foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
			{
				var doc = _indexReader.Document(scoreDoc.Doc);
				var vicinityId = long.Parse(doc.Get(VicinityCacheIndexMap.FieldNames.ID));
				result.Add(this[vicinityId]);
			}

			return result;
		}

		#endregion

	    #region Overrides

	    protected override void IndexLoadedData(IEnumerable<Vicinity> all)
	    {
		    _loadedVicinities = all.ToList();
		    base.IndexLoadedData(_loadedVicinities);

		    BuildHierarchy();
		    BuildLuceneIndex();
	    }

	    #endregion

	    #region Private helper methods

	    private void BuildHierarchy()
	    {
		    _rootVicinity = new Vicinity
		                    {
			                    ID = 0,
			                    Name = "rootVicinity",
			                    AlternativeNames = "",
			                    Type = VicinityType.HierarchyNode,
			                    Children = new List<Vicinity>()
		                    };

		    foreach (var vicinity in _loadedVicinities)
		    {
			    // Set parent
			    if (vicinity.ParentID.HasValue)
				    vicinity.Parent = CacheData[vicinity.ParentID.Value];

			    // Set children
			    if (vicinity.Parent != null)
			    {
				    if (vicinity.Parent.Children == null)
				    {
					    vicinity.Parent.Children = new List<Vicinity>();
				    }

				    vicinity.Parent.Children.Add(vicinity);
			    }
			    else
			    {
				    _rootVicinity.Children.Add(vicinity);
			    }
		    }

		    CacheData.TryAdd(_rootVicinity.ID, _rootVicinity);
	    }

	    private void BuildLuceneIndex()
	    {
		    _indexDirectory = new RAMDirectory();
		    var indexMap = new VicinityCacheIndexMap();

		    using (var indexWriter = new IndexWriter(_indexDirectory, new LucenePersianAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED))
		    {
			    foreach (var vicinity in _loadedVicinities)
			    {
				    indexMap.PopulateDocument(vicinity);
				    indexWriter.AddDocument(indexMap.GetDocument());
			    }

			    indexWriter.Commit();
		    }

		    _indexSearcher = new IndexSearcher(_indexDirectory);
		    _indexReader = _indexSearcher.IndexReader;
	    }

	    #endregion
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class VicinityCacheLoader : ICacheLoader<Vicinity>
    {
        public IEnumerable<Vicinity> LoadAll()
        {
            using (var db = new Db())
            {
               return db.Vicinities.ToList();
            }
        }
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class VicinityCacheValueCopier : ICacheValueCopier<Vicinity>
    {
        public Vicinity Copy(Vicinity original)
        {
            return Vicinity.Copy(original);
        }
    }

    [Component]
    [ComponentCache(typeof(DefaultComponentCache))]
    public class VicinityCacheKeyMapper : ICacheKeyMapper<long, Vicinity>
    {
        public long MapKey(Vicinity value)
        {
            return value.ID;
        }
    }
}
