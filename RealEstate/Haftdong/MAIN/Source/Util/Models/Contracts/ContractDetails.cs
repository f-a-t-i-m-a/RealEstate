using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [AutoMapperConfig]
    public class ContractDetails
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ContractState State { get; set; }

        public string Portion { get; set; }
        public string District { get; set; }
        public string RegistrationZone { get; set; }
        public string OwnershipEvidenceSerialNumber { get; set; }
        public string NotaryPublicPageNumber { get; set; }
        public string NotaryPublic { get; set; }
        public string PublicSpace { get; set; }
        public string Description { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        public decimal? EstateArea { get; set; }
        public decimal? UnitArea { get; set; }

        public CustomerDetails Seller { get; set; }
        public CustomerDetails Buyer { get; set; }
        public RequestSummary RequestSummary { get; set; }
        public SupplyDetails SupplyDetails { get; set; }



        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Contract, ContractDetails>()
                .IgnoreUnmappedProperties();
        }
    }
}