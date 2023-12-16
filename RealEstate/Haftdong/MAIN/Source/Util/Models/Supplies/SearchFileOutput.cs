using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    public class SearchFileOutput
    {
        public PagedListOutput<SupplySummary> Supplies { get; set; }
    }
}
