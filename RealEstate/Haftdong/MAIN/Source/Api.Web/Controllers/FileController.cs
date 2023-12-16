using System.Web.Http;
using Compositional.Composer;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("files")]
    public class FileController : ExtendedApiController
    {
        #region Action methods

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public SearchFileOutput Search(SearchFileInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = FileUtil.Search(searchInput, false, false);

            return new SearchFileOutput
            {
                Supplies = result
            };
        }

        [HttpPost, Route("myfiles")]
        [SkipUserActivityLogging]
        public SearchFileOutput MyFiles(SearchFileInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = FileUtil.Search(searchInput, false, true);

            return new SearchFileOutput
            {
                Supplies = result
            };
        }

        [HttpPost, Route("mypublicfiles")]
        [SkipUserActivityLogging]
        public SearchFileOutput MyPublicFiles(SearchFileInput searchInput)
        {
            searchInput.IsPublic = true;
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = FileUtil.Search(searchInput, false, true);

            return new SearchFileOutput
            {
                Supplies = result
            };
        }

        [HttpPost, Route("searchinmap")]
        [SkipUserActivityLogging]
        public GeoSearchResult SearchInMap(SearchFileInput searchInput)
        {
            var mapResult = new GeoSearchResult();
            if (searchInput.Bounds == null)
                return mapResult;

            searchInput.StartIndex = 0;
            searchInput.PageSize = MaxPageSize;

            var boundingBox = new LatLngBounds
            {
                SouthWest = new LatLng {Lat = searchInput.Bounds.SouthLat, Lng = searchInput.Bounds.WestLng},
                NorthEast = new LatLng {Lat = searchInput.Bounds.NorthLat, Lng = searchInput.Bounds.EastLng}
            };
            var boundingBoxArea = boundingBox.GetArea();
            mapResult.LargestContainedRect = boundingBox.GetLargestContainedRect();
            mapResult.LargestContainedRectArea = mapResult.LargestContainedRect.GetArea();
            mapResult.MinimumDistinguishedArea = mapResult.LargestContainedRectArea/20;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (boundingBoxArea == 0)
                return mapResult;

            FileUtil.SearchInMap(searchInput, mapResult, false, false);

            return mapResult;
        }

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        #endregion

        #region Private helper methods 

        #endregion
    }
}