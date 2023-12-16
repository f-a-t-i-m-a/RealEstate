using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    public class SearchVicinityInput
    {
        public string SearchText { get; set; }
        public bool CanContainPropertyRecordsOnly { get; set; }
        public ObjectId? ParentId { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public PropertySortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
