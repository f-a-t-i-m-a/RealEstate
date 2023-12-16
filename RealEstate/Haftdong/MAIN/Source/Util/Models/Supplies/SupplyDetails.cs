using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    [AutoMapperConfig]
    public class SupplyDetails : SupplySummary
    {
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }
        
        public bool MortgageAndRentConvertible { get; set; }
        public string AdditionalRentalComments { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }
        public PropertyDetails PropertyDetail { get; set; }
        public RequestDetails Request { get; set; }
        public string SwapAdditionalComments { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Supply, SupplyDetails>()
                .IncludeBase<Supply, SupplySummary>()
                .IgnoreUnmappedProperties();
        }
    }
}