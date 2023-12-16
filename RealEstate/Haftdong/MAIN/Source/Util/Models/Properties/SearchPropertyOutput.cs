using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    public class SearchPropertyOutput
    {
        public PagedListOutput<PropertySummary> Properties { get; set; }
    }
}
