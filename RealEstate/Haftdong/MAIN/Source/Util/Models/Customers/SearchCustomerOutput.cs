using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Customers
{
    public class SearchCustomersOutput
    {
        public PagedListOutput<CustomerSummary> Customers { get; set; }
    }
}
