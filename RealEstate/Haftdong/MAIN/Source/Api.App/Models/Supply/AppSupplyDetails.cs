using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Request;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSupplyDetails : AppSupplySummary
    {
        public bool IsArchived { get; set; }

        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }
        
        public bool MortgageAndRentConvertible { get; set; }
        public string AdditionalRentalComments { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }
        public AppRequestDetails Request { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Supply.Supply, AppSupplyDetails>()
                .IncludeBase<Domain.Supply.Supply, AppSupplySummary>()
                .IgnoreUnmappedProperties();
        }
    }
}