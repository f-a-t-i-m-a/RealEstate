using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    [AutoMapperConfig]
    public class UpdateSupplyInput
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
        public string AdditionalRentalComments { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public NewContactMethodCollectionInput OwnerContact { get; set; }
        public NewContactMethodCollectionInput AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateSupplyInput, Supply>()
                .IgnoreUnmappedProperties();
        }
    }
}
