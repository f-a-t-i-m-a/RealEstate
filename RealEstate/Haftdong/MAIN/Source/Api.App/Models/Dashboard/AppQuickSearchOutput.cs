using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Dashboard
{
    [TsClass]
    public class AppQuickSearchOutput
    {
         public PagedListOutput<AppSearchResultSummary> SearchResults { get; set; }
    }
}