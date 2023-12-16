using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Dashboard
{
    [TsClass]
    public class QuickSearchInput
    {
        public string Text { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public List<EntityType> DataTypes { get; set; }
        public DashboardSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
