using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Vicinity
{
    [TsClass]
    public class AppSearchVicinityInput
    {
        public string SearchText { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public PropertySortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
