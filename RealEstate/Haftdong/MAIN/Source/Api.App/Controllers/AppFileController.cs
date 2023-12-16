using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Map;
using JahanJooy.RealEstateAgency.Api.App.Models.Property;
using JahanJooy.RealEstateAgency.Api.App.Models.Supply;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("files")]
    public class AppFileController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public AppSearchFileOutput Search(AppSearchFileInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var input = Mapper.Map<SearchFileInput>(searchInput);
            var result = FileUtil.Search(input, false, searchInput.MineOnly);

            var appSupplies = new List<AppSupplySummary>();
            result.PageItems?.ForEach(s =>
            {
                var appSupplySummary = Mapper.Map<AppSupplySummary>(s);
                var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", s.ID);
                appSupplySummary.PropertyDetail = DbManager.Property.Find(propertyFilter)
                    .Project(p => Mapper.Map<AppPropertyDetails>(p))
                    .SingleOrDefaultAsync()
                    .Result;

                if (appSupplySummary.PropertyDetail?.Vicinity != null)
                {
                    appSupplySummary.PropertyDetail.Vicinity.CompleteName = VicinityUtil.GetFullName(appSupplySummary.PropertyDetail.Vicinity.ID);
                }

                appSupplies.Add(appSupplySummary);
            });

            var appResult = new PagedListOutput<AppSupplySummary>
            {
                PageItems = appSupplies,
                PageNumber = result.PageNumber,
                TotalNumberOfItems = result.TotalNumberOfItems,
                TotalNumberOfPages = result.TotalNumberOfPages
            };

            return new AppSearchFileOutput
            {
                SupplyPagedList = appResult
            };
        }

        [HttpPost, Route("searchinmap")]
        [SkipUserActivityLogging]
        public AppGeoSearchResult SearchInMap(AppSearchFileInput searchInput)
        {
            var mapResult = new GeoSearchResult();
            if (searchInput.Bounds == null)
                return Mapper.Map<AppGeoSearchResult>(mapResult);

            searchInput.StartIndex = 0;
            searchInput.PageSize = MaxPageSize;
            var input = Mapper.Map<SearchFileInput>(searchInput);

            var boundingBox = new LatLngBounds
            {
                SouthWest = new LatLng { Lat = searchInput.Bounds.SouthLat, Lng = searchInput.Bounds.WestLng },
                NorthEast = new LatLng { Lat = searchInput.Bounds.NorthLat, Lng = searchInput.Bounds.EastLng }
            };
            var boundingBoxArea = boundingBox.GetArea();
            mapResult.LargestContainedRect = boundingBox.GetLargestContainedRect();
            mapResult.LargestContainedRectArea = mapResult.LargestContainedRect.GetArea();
            mapResult.MinimumDistinguishedArea = mapResult.LargestContainedRectArea / 20;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (boundingBoxArea == 0)
                return Mapper.Map<AppGeoSearchResult>(mapResult);

            FileUtil.SearchInMap(input, mapResult, false, false);

            AppGeoSearchResult result = Mapper.Map<AppGeoSearchResult>(mapResult);

            return result;
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}