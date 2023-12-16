using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Dashboard;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Models.Dashboard;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("dashboard")]
    public class AppDashboardController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public DashBoardUtil DashBoardUtil { get; set; }

        private int MaxPageSize = 100;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("userquicksearch")]
        [SkipUserActivityLogging]
        public AppQuickSearchOutput UserQuickSearch(AppQuickSearchInput appInput)
        {
            if (appInput.PageSize == 0)
                appInput.PageSize = DefaultPageSize;
            else if (appInput.PageSize < 0 || appInput.PageSize > MaxPageSize)
                appInput.PageSize = MaxPageSize;

            var input = Mapper.Map<QuickSearchInput>(appInput);
            var response = DashBoardUtil.UserQuickSearch(input);

            var results = PrepareOutput(response.Documents);

            return new AppQuickSearchOutput
            {
                SearchResults = new PagedListOutput<AppSearchResultSummary>
                {
                    PageItems = results,
                    PageNumber = (appInput.StartIndex/ appInput.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/appInput.PageSize)
                }
            };
        }

        #endregion

        #region Private helper methods 

        private List<AppSearchResultSummary> PrepareOutput(IEnumerable<SearchIE> documents)
        {
            var results = new List<AppSearchResultSummary>();

            documents.ForEach(d =>
            {
                results.Add(new AppSearchResultSummary
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