using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    public class AppSearchRequestOutput
    {
        public PagedListOutput<AppRequestSummary> RequestPagedList { get; set; }
    }
}
