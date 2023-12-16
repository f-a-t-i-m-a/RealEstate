using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Vicinity
{
    public class AppSearchVicinityOutput
    {
        public PagedListOutput<AppVicinitySummary> VicinityPagedList { get; set; }
    }
}
