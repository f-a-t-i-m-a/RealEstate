using JahanJooy.Common.Util.ApiModel.Pagination;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.UserActivities
{
    [TsClass]
    public class SearchOutput
    {
        public PagedListOutput<UserActivitySummary> UserActivities { get; set; }
    }
}