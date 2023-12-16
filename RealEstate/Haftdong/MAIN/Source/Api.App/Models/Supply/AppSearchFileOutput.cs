using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    public class AppSearchFileOutput
    {
        public PagedListOutput<AppSupplySummary> SupplyPagedList { get; set; }
    }
}
