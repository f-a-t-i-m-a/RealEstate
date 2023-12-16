using System;
using System.Collections.Generic;
using System.Web.Http;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Dashboard;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using MongoDB.Bson;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("dashboard")]
    public class DashboardController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }
        [ComponentPlug]
        public DashBoardUtil DashBoardUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("quicksearch")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [SkipUserActivityLogging]
        public QuickSearchOutput QuickSearch(QuickSearchInput input)
        {
            if (input.PageSize == 0)
                input.PageSize = DefaultPageSize;
            else if (input.PageSize < 0 || input.PageSize > MaxPageSize)
                input.PageSize = MaxPageSize;

            var response = ElasticManager.Client.Search<SearchIE>(si => si
                .Index(ElasticManager.Index)
                .Type(Types.SearchType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    query &= q.MatchPhrasePrefix(m => m.Field(o => o.IndexedText).Query(input.Text.ToLower()));
                    query &= q.Terms(c => c.Field(p => p.DataType).Terms(input.DataTypes));
                    return query;
                })
                .From(input.StartIndex)
                .Take(input.PageSize)
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while quick searching, debug information: {0}",
                    response.DebugInformation);
            }

            var results = PrepareOutput(response.Documents);

            return new QuickSearchOutput
            {
                SearchResults = new PagedListOutput<SearchResultSummary>
                {
                    PageItems = results,
                    PageNumber = (input.StartIndex/input.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/input.PageSize)
                }
            };
        }

        [HttpPost, Route("userquicksearch")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [SkipUserActivityLogging]
        public QuickSearchOutput UserQuickSearch(QuickSearchInput input)
        {
            if (input.PageSize == 0)
                input.PageSize = DefaultPageSize;
            else if (input.PageSize < 0 || input.PageSize > MaxPageSize)
                input.PageSize = MaxPageSize;

            var response = DashBoardUtil.UserQuickSearch(input);
            var results = PrepareOutput(response.Documents);
            return new QuickSearchOutput
            {
                SearchResults = new PagedListOutput<SearchResultSummary>
                {
                    PageItems = results,
                    PageNumber = (input.StartIndex/input.PageSize) + 1,
                    TotalNumberOfItems = (int)response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/input.PageSize)
                }
            };
        }

        #endregion

        #region Private helper methods 

        private List<SearchResultSummary> PrepareOutput(IEnumerable<SearchIE> documents)
        {
            var results = new List<SearchResultSummary>();

            documents.ForEach(d =>
            {
                results.Add(new SearchResultSummary
                {
                    ID = ObjectId.Parse(d.ID),
                    DataType = d.DataType,
                    Title = d.Title,
                    DisplayText = d.DisplayText
                });
            });

            return results;
        }

        #endregion
    }
}