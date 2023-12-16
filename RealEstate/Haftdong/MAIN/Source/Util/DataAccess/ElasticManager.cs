using System;
using System.Collections.Generic;
using System.Configuration;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstateAgency.Util.Indexing;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Util.DataAccess
{
    [Contract]
    [Component]
    public class ElasticManager
    {
        private const string ConnectionStringNameUri = "ElasticUri";
        private const string ConnectionStringNameIndex = "ElasticIndex";

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        private static ElasticClient CurrentClient { get; set; }
        private static string CurrentIndex { get; set; }

        public ElasticManager()
        {
            var uriSettings = ConfigurationManager.ConnectionStrings[ConnectionStringNameUri];
            if (uriSettings == null)
                throw new ConfigurationErrorsException(
                    "Elastic Search URI Connection String is not configured. A connection string named '" +
                    ConnectionStringNameUri + "' should be present in the application configuration.");

            var uri = uriSettings.ConnectionString;
            if (uri == null)
                throw new ConfigurationErrorsException(
                    "Elastic Search URI Connection String (named '" + ConnectionStringNameUri + "') is empty. ");

            var indexSettings = ConfigurationManager.ConnectionStrings[ConnectionStringNameIndex];
            if (indexSettings == null)
                throw new ConfigurationErrorsException(
                    "Elastic Search Index Connection String is not configured. A connection string named '" +
                    ConnectionStringNameIndex + "' should be present in the application configuration.");

            CurrentIndex = indexSettings.ConnectionString;
            if (CurrentIndex == null)
                throw new ConfigurationErrorsException(
                    "Elastic Search Index Connection String (named '" + ConnectionStringNameIndex + "') is empty. ");

            var node = new Uri(uri);
            var settings = new ConnectionSettings(node);

            if (ApplicationEnvironmentUtil.Type != ApplicationEnvironmentType.Production)
                settings.DisableDirectStreaming();

            CurrentClient = new ElasticClient(settings);
        }

        public void IndexEntity<TEntity, TEntityIE, TSearchEntityIE>(string type, TEntity entity)
            where TEntityIE : class where TSearchEntityIE : class
        {
            var entityIe = Composer.GetComponent<IIndexMapper<TEntity, TEntityIE>>().Map(entity);
            var response = CurrentClient.Index(entityIe, i => i
                .Index(CurrentIndex)
                .Type(type)
                .Refresh());

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while indexing entity type {0} with ID {1}, debug information: {2}",
                    typeof(TEntity),
                    entity.GetType().GetProperty("ID")?.GetValue(entity) + "" +
                    entity.GetType().GetProperty("Id")?.GetValue(entity), response.DebugInformation);
            }

            var searchIe = Composer.GetComponent<ISearchIndexMapper<TEntity, TSearchEntityIE>>()?.SearchMap(entity);
            if (searchIe != null)
            {
                response = CurrentClient.Index(searchIe, i => i
                    .Index(CurrentIndex)
                    .Type(Types.SearchType)
                    .Refresh());

                if (!response.IsValid)
                {
                    ApplicationStaticLogs.ElasticLog.ErrorFormat(
                        "An error occured while indexing entity type {0} with ID {1}, debug information: {2}",
                        typeof(TEntity), entity.GetType().GetProperty("ID")?.GetValue(entity), response.DebugInformation);
                }
            }
        }

        public void ReIndexEntity<TEntity, TEntityIE, TSearchEntityIE>(string type, TEntity entity, string id)
            where TEntityIE : class where TEntity : class where TSearchEntityIE : class
        {
            var entityIe = Composer.GetComponent<IIndexMapper<TEntity, TEntityIE>>().Map(entity);
            var response = CurrentClient.Update<TEntity, TEntityIE>(new DocumentPath<TEntity>(id), e => e
                .Index(CurrentIndex)
                .Type(type)
                .Doc(entityIe)
                .Refresh());

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while reindexing entity type {0} with ID {1}, debug information: {2}",
                    typeof(TEntity),
                    entity.GetType().GetProperty("ID")?.GetValue(entity) + "" +
                    entity.GetType().GetProperty("Id")?.GetValue(entity), response.DebugInformation);
            }

            var searchIe = Composer.GetComponent<ISearchIndexMapper<TEntity, TSearchEntityIE>>()?.SearchMap(entity);
            if (searchIe != null)
            {
                response = CurrentClient.Update<TEntity, TSearchEntityIE>(new DocumentPath<TEntity>(id), e => e
                    .Index(CurrentIndex)
                    .Type(Types.SearchType)
                    .Doc(searchIe)
                    .Refresh());

                if (!response.IsValid)
                {
                    ApplicationStaticLogs.ElasticLog.ErrorFormat(
                        "An error occured while reindexing entity type {0} with ID {1}, debug information: {2}",
                        typeof(TEntity), entity.GetType().GetProperty("ID")?.GetValue(entity), response.DebugInformation);
                }
            }
        }

        public void IndexEntityMany<TEntity, TEntityIE, TSearchEntityIE>(string type, List<TEntity> entities)
            where TEntityIE : class where TSearchEntityIE : class
        {
            List<TEntityIE> entityIes = new List<TEntityIE>();
            entities.ForEach(e => { entityIes.Add(Composer.GetComponent<IIndexMapper<TEntity, TEntityIE>>().Map(e)); });

            CurrentClient.IndexMany(entityIes, CurrentIndex, type);

            List<TSearchEntityIE> searchIes = new List<TSearchEntityIE>();
            entities.ForEach(
                e =>
                {
                    searchIes.Add(Composer.GetComponent<ISearchIndexMapper<TEntity, TSearchEntityIE>>()?.SearchMap(e));
                });

            CurrentClient.IndexMany(searchIes, CurrentIndex, Types.SearchType);
        }

        public ElasticClient Client => CurrentClient;
        public string Index => CurrentIndex;
    }
}