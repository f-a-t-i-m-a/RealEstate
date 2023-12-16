using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    public class AppSearchPropertyOutput
    {
        public PagedListOutput<AppPropertySummary> PropertyPagedList { get; set; }
    }
}
