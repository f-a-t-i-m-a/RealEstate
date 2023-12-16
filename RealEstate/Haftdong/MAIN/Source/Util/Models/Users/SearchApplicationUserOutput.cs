using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    public class SearchApplicationUserOutput
    {
        public PagedListOutput<ApplicationUserSummary> Users { get; set; }
    }
}
