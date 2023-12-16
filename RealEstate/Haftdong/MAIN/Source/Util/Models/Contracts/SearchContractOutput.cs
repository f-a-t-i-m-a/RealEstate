using JahanJooy.Common.Util.ApiModel.Pagination;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    public class SearchContractOutput
    {
        public PagedListOutput<ContractSummary> Contracts { get; set; }
    }
}
