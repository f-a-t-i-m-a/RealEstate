using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using Compositional.Composer.Cache;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.CacheMongo;
using JahanJooy.RealEstateAgency.Util.CacheMongo.Components;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Utils.Base;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Util.VicinityCache
{
    [Component]
    [ComponentCache(typeof (DefaultComponentCache))]
    public class VicinityCache : AutoLoadCacheMongo<ObjectId, Vicinity>, IVicinityCache
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

        public List<Vicinity> Search(string query, bool canContainPropertyRecordsOnly,
            int startIndex, int pageSize, ObjectId? rootVicinityId = null, bool includeDisabled = false)
        {
            EnsureLoaded();

            var rootQuery = new BooleanQuery();

            if (rootVicinityId.HasValue && rootVicinityId.Value != ObjectId.Empty)
            {
                var scopeQuery =
                    new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.ParentsIDs, rootVicinityId.Value.ToString()));
                rootQuery.Add(scopeQuery, Occur.MUST);
            }

            if (!includeDisabled)
            {
                var enabledQuery = new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.Enabled, true.ToString()));
                rootQuery.Add(enabledQuery, Occur.MUST);
            }

            var nameQuery = new BooleanQuery();
            rootQuery.Add(nameQuery, Occur.MUST);

            foreach (var inputTerm in UserSearchQueryUtils.TokenizeUserQuery(query))
            {
                if (string.IsNullOrWhiteSpace(inputTerm))
                    continue;

                var inputTermQuery = new BooleanQuery
                {
                    {
                        new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.Name, inputTerm)) {Boost = 1.5f},
                        Occur.SHOULD
                    },
                    {
                        new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.AlternativeNames, inputTerm))
                        {
                            Boost = 1.4f
                        },
                        Occur.SHOULD
                    },
                    {
                        new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.AdditionalSearchText, inputTerm))
                        {
                            Boost = 1.3f
                        },
                        Occur.SHOULD
                    },
                    {
                        new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.LocalizedType, inputTerm)) {Boost = 0.5f},
                        Occur.SHOULD
                    },
                    {
                        new PrefixQuery(new Term(VicinityCacheIndexMap.FieldNames.ParentsSearchText, inputTerm))
                        {
                            Boost = -1f
                        },
                        Occur.SHOULD
                    }
                };

                nameQuery.Add(inputTermQuery, Occur.MUST);
            }

            if (canContainPropertyRecordsOnly)
            {
                rootQuery.Add(
                    new TermQuery(new Term(VicinityCacheIndexMap.FieldNames.CanContainPropertyRecords, true.ToString())),
                    Occur.MUST);
            }

            var result = new List<Vicinity>();

            TopDocs topDocs = _indexSearcher.Search(rootQuery, startIndex + pageSize);
            if (topDocs.ScoreDocs.Length < startIndex)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var scoreDoc in topDocs.ScoreDocs.Skip(startIndex))
            {
                var doc = _indexReader.Document(scoreDoc.Doc);
                var vicinityId = ObjectId.Parse(doc.Get(VicinityCacheIndexMap.FieldNames.ID));
                result.Add(this[vicinityId]);
            }

            return result;
        }

        public IEnumerable<Vicinity> GetParents(ObjectId vicinityId)
        {
            return GetParentsInclusive(vicinityId).Skip(1);
        }

        public IEnumerable<Vicinity> GetParentsInclusive(ObjectId vicinityId)
        {
            if (!CacheData.ContainsKey(vicinityId))
                return null;

            return CacheData[vicinityId].GetParentsInclusive().Select(v => Copier.Copy(v));
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
                ID = ObjectId.Empty,
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

            using (
                var indexWriter = new IndexWriter(_indexDirectory, new LucenePersianAnalyzer(), true,
                    IndexWriter.MaxFieldLength.LIMITED))
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
    [ComponentCache(typeof (DefaultComponentCache))]
    public class VicinityCacheLoader : ICacheLoaderMongo<Vicinity>
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public IEnumerable<Vicinity> LoadAll()
        {
            var filter = new BsonDocument();
            return AsyncHelpers.RunSync(() => DbManager.Vicinity.Find(filter).ToListAsync());
        }
    }

    [Component]
    [ComponentCache(typeof (DefaultComponentCache))]
    public class VicinityCacheValueCopier : ICacheValueCopierMongo<Vicinity>
    {
        public Vicinity Copy(Vicinity original)
        {
            return Vicinity.Copy(original);
        }
    }

    [Component]
    [ComponentCache(typeof (DefaultComponentCache))]
    public class VicinityCacheKeyMapper : ICacheKeyMapperMongo<ObjectId, Vicinity>
    {
        public ObjectId MapKey(Vicinity value)
        {
            return value.ID;
        }
    }
}