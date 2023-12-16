using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Resources;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.VicinityCache;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("vicinities")]
    public class AppVicinityController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Vicinity action methods

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public AppSearchVicinityOutput Search(AppSearchVicinityInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var response = VicinityCache.Search(searchInput.SearchText, true, searchInput.StartIndex, searchInput.PageSize);
            var allResponses = VicinityCache.Search(searchInput.SearchText, true, 0, MaxPageSize);

            var vicinitySummaries = response.Select(Mapper.Map<AppVicinitySummary>).ToList();

            vicinitySummaries.ForEach(v =>
            {
                var parentId = v.ParentID;
                while (parentId.HasValue)
                {
                    var parent = VicinityCache[parentId.Value];
                    v.CompleteName += ", " +
                                      StaticEnumResources.ResourceManager.GetString(nameof(VicinityType) + "_" +
                                                                                    parent.Type) + " " + parent.Name;
                    parentId = parent.ParentID;
                }
                if (v.CompleteName != null && v.CompleteName.Length >= 2)
                    v.CompleteName = v.CompleteName.Substring(2);

               
            });

            var result = new PagedListOutput<AppVicinitySummary>
            {
                PageItems = vicinitySummaries,
                PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                TotalNumberOfItems = allResponses.Count,
                TotalNumberOfPages = (int) Math.Ceiling((decimal)allResponses.Count/searchInput.PageSize)
            };

            return new AppSearchVicinityOutput
            {
                VicinityPagedList = result
            };
        }

        #endregion

        #region Private methods

        #endregion
    }
}