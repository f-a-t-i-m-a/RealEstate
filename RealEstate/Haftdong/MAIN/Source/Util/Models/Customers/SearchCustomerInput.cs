using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Customers
{
    [TsClass]
    public class SearchCustomerInput
    {
        public string DisplayName { get; set; }
        public long IntentionOfVisit { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsDeleted { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public CustomerSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
