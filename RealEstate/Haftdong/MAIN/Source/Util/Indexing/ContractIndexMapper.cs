using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Contract;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class ContractIndexMapper : IIndexMapper<Contract, ContractIE>
    {
        public ContractIE Map(Contract entity)
        {
            var description = entity.PropertyReference?.UsageType + " " + entity.PropertyReference?.PropertyType
                             + " " + entity.SupplyReference?.IntentionOfOwner + " " + entity.State;
            return new ContractIE
            {
                ID = entity.ID.ToString(),
                ContractDate = entity.ContractDate.Ticks,
                TrackingID = entity.TrackingID,
                Description = description,
                CreatedByID = entity.CreatedByID.ToString(),
                Rent = entity.Rent,
                Mortgage = entity.Mortgage,
                TotalPrice = entity.TotalPrice
            };
        }
    }
}