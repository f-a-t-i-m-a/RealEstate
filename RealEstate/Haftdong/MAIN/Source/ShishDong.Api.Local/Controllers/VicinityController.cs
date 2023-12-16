using System.Web.Http;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.ShishDong.Api.Local.Base;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;

namespace JahanJooy.RealEstateAgency.ShishDong.Api.Local.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("vicinities")]
    public class VicinityController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        #endregion

        #region Vicinity action methods

        [HttpPost, Route("findbypoint")]
        [SkipUserActivityLogging]
        public VicinityReference FindByPoint(SearchVicinityByPointInput input)
        {
            return VicinityUtil.FindByPoint(input);
        }

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public SearchVicinityOutput Search(SearchVicinityInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            return VicinityUtil.Search(searchInput);
        }

        #endregion

        #region Private methods

        #endregion
    }
}