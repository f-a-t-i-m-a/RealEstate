using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using Compositional.Composer.Cache;
using JahanJooy.Common.Util.Collections;
using log4net;

namespace JahanJooy.Common.Util.Search
{
    [Contract]
    [Component]
    [ComponentCache(typeof (ContractAgnosticComponentCache))]
    public class LuceneIndexManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LuceneIndexManager));

        private static readonly Dictionary<string, LuceneIndexWrapper> Indexes;
        private static readonly object IndexesLock;

        [ComponentPlug]
        public IComposer Composer { get; set; }

        #region Initialization

        static LuceneIndexManager()
        {
            Indexes = new Dictionary<string, LuceneIndexWrapper>();
            IndexesLock = new object();
        }

        public static void RegisterConfigurationKeys(string indexId)
        {
            LuceneIndexWrapper.RegisterConfigurationKeys(indexId);
        }

        public void InitializeIndex(string indexId)
        {
            Log.InfoFormat("Initializing index id {0}", indexId);

            // Re-register configuration keys in case a client haven't called RegisterConfigurationKeys prior to this method
            RegisterConfigurationKeys(indexId);

            lock (IndexesLock)
            {
                if (Indexes.ContainsKey(indexId))
                    return;

                var luceneIndexWrapper = new LuceneIndexWrapper(indexId);
                Composer.InitializePlugs(luceneIndexWrapper);

                Indexes[indexId] = luceneIndexWrapper;
            }
        }

        public void DeleteFilesAndRebuild(string indexId)
        {
            // TODO
        }

        #endregion

        #region Public services

        public LuceneIndexWriterRef AcquireWriter(string indexId)
        {
            return GetWrapper(indexId).AcquireWriter();
        }

        public LuceneIndexSearcherRef AcquireSearcher(string indexId)
        {
            return GetWrapper(indexId).AcquireSearcher();
        }

        public void DeleteAllDocuments(string indexId)
        {
            GetWrapper(indexId).DeleteAllDocuments();
        }

        public void Commit(string indexId)
        {
            // TODO: If OutOfMemoryException is thrown, shutdown and create another wrapper
            GetWrapper(indexId).Commit();
        }

        public void Rollback(string indexId)
        {
            // TODO: If OutOfMemoryException is thrown, shutdown and create another wrapper
            GetWrapper(indexId).Rollback();
        }

        public void Optimize(string indexId)
        {
            // TODO: If OutOfMemoryException is thrown, shutdown and create another wrapper
            GetWrapper(indexId).Optimize();
        }

        public void CommitAll()
        {
            Log.Debug("Committing all of the indexes");
            Indexes.Keys.ForEach(Commit);
        }

        public void OptimizeAll()
        {
            Log.Debug("Optimizing all of the indexes");
            Indexes.Keys.ForEach(Optimize);
        }

        public void ClearErrors(string indexId)
        {
            GetWrapper(indexId).ClearErrors();
        }

        public void ClearAllErrors()
        {
            Indexes.Keys.ForEach(ClearErrors);
        }

        public LuceneIndexHealthStatus GetHealthStatus(string indexId)
        {
            return GetWrapper(indexId).GetHealthStatus();
        }

        public List<LuceneIndexHealthStatus> GetAllHealthStatuses()
        {
            return Indexes.Values.Select(iw => iw.GetHealthStatus()).ToList();
        }

        public LuceneIndexStatistics GetStatistics(string indexId)
        {
            return GetWrapper(indexId).GetStatistics();
        }

        public List<Exception> GetErrors(string indexId)
        {
            return GetWrapper(indexId).AllErrors;
        }

        public List<LuceneIndexStatistics> GetAllStatistics()
        {
            return Indexes.Values.Select(iw => iw.GetStatistics()).ToList();
        }

        #endregion

        #region Public properties

        public IEnumerable<string> IndexKeys
        {
            get { return Indexes.Keys; }
        }

        public bool HasErrors
        {
            get { return Indexes.Values.Any(iw => iw.HasErrors); }
        }

        #endregion

        #region Private helper methods

        private static LuceneIndexWrapper GetWrapper(string indexId)
        {
            if (!Indexes.ContainsKey(indexId))
                throw new InvalidOperationException("Index id " + indexId +
                                                    " is not initialized. InitializeIndex should be called prior to any usage of the index.");

            return Indexes[indexId];
        }

        #endregion
    }
}