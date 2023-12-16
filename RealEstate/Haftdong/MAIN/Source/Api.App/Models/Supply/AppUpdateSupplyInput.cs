using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppUpdateSupplyInput
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppUpdateSupplyInput, Domain.Supply.Supply>()
                .IgnoreUnmappedProperties();
        }
    }
}
