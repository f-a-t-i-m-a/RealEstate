using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class SupplyReference
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime CreationTime { get; set; }
        public ObjectId CreatedByID { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public SupplyState State { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPublic { get; set; }

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }

        public bool HasTransferableLoan { get; set; }
        public decimal? TransferableLoanAmount { get; set; }
        public bool MortgageAndRentConvertible { get; set; }
        public string AdditionalRentalComments { get; set; }
        public decimal? MinimumMortgage { get; set; }
        public decimal? MinimumRent { get; set; }

        public bool? ContactToOwner { get; set; }
        public bool? ContactToAgency { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Supply, SupplyReference>()
                .ForMember(s => s.ContactToOwner, opt => opt.MapFrom(su => su.OwnerCanBeContacted == true))
                .ForMember(s => s.ContactToAgency, opt => opt.MapFrom(su => su.OwnerCanBeContacted == false))
                .IgnoreUnmappedProperties();
        }
    }
}