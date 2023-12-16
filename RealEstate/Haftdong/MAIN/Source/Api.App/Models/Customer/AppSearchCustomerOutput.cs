using JahanJooy.Common.Util.ApiModel.Pagination;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [TsClass]
    public class AppSearchCustomersOutput
    {
        public PagedListOutput<AppCustomerSummary> CustomerPagedList { get; set; }
    }
}
