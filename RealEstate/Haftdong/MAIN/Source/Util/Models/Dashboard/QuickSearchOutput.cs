using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Dashboard
{
    [TsClass]
    public class QuickSearchOutput
    {
         public PagedListOutput<SearchResultSummary> SearchResults { get; set; }
    }
}