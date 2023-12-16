using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    public class SearchVicinityOutput
    {
        public PagedListOutput<VicinitySummary> Vicinities { get; set; }
    }
}
