using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    public class SearchRequestOutput
    {
        public PagedListOutput<RequestSummary> Requests { get; set; }
        public bool ExtendMenu { get; set; }
    }
}
