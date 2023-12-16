using System;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Base;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    public class SearchContractInput
    {
        public UsageType? UsageType { get; set; }
        public PropertyType? PropertyType { get; set; }
        public IntentionOfOwner? IntentionOfOwner { get; set; }
        public ContractState? State { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public ContractSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
