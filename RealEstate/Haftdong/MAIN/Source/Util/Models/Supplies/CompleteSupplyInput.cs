using JahanJooy.RealEstateAgency.Domain.Workflows;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    public class CompleteSupplyInput
    {
        public string ID { get; set; }
        public SupplyCompletionReason Reason { get; set; }
    }
}