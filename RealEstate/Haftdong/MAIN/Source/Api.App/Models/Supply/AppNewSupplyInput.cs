using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewSupplyInput
    {
        public IntentionOfOwner IntentionOfOwner { get; set; }

        #region Sales price

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        #endregion
        public ObjectId PropertyId { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewSupplyInput, Domain.Supply.Supply>()
                .IgnoreUnmappedProperties();
        }
    }
}
