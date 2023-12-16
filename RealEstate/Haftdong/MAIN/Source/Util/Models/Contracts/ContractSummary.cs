using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [TsClass]
    [AutoMapperConfig]
    public class ContractSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime ContractDate { get; set; }
        public ContractState State { get; set; }
        public int? TrackingID { get; set; }
        public SupplySummary SupplySummary { get; set; }
        public PropertySummary PropertySummary { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public CustomerSummary Seller { get; set; }
        public CustomerSummary Buyer { get; set; }
        public ObjectId CreatedByID { get; set; }
        public string CreatorFullName { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Contract, ContractSummary>()
                .ForMember(cs=>cs.Seller, opt=>opt.MapFrom(c=>c.SellerReference))
                .ForMember(cs=>cs.Buyer, opt=>opt.MapFrom(c=>c.BuyerReference))
                .ForMember(cs=>cs.SupplySummary, opt=>opt.MapFrom(c=>c.SupplyReference))
                .ForMember(cs=>cs.PropertySummary, opt=>opt.MapFrom(c=>c.PropertyReference))
                .IgnoreUnmappedProperties();

        }
     
    }
}
