using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [TsClass]
    [AutoMapperConfig]
    public class NewContractInput
    {
        public DateTime ContractDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ObjectId?  PropertyID { get; set; }
        public ObjectId?  SupplyID { get; set; }
        public string  SellerID { get; set; }
        public string  BuyerID { get; set; }
        public ObjectId? RequestID { get; set; }
        public UsageType? UsageType { get; set; }
        public PropertyType PropertyType { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public string LicencePlate { get; set; }
        public string Address { get; set; }
        public string Portion { get; set; }
        public string District { get; set; }
        public string RegistrationZone { get; set; }
        public string OwnershipEvidenceSerialNumber { get; set; }
        public string NotaryPublicPageNumber { get; set; }
        public string NotaryPublic { get; set; }
        public string PublicSpace { get; set; }
        public string Description { get; set; }

        public decimal? ContractTotalPrice { get; set; }
        public decimal? ContractMortgage { get; set; }
        public decimal? ContractRent { get; set; }
        public decimal? ContractEstateArea { get; set; }
        public decimal? ContractUnitArea { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewContractInput, Contract>()
                 .ForMember(c => c.TotalPrice, opt => opt.MapFrom(nci =>nci.ContractTotalPrice))
                 .ForMember(c => c.Mortgage, opt => opt.MapFrom(nci =>nci.ContractMortgage))
                 .ForMember(c => c.Rent, opt => opt.MapFrom(nci =>nci.ContractRent))
                 .ForMember(c => c.UnitArea, opt => opt.MapFrom(nci =>nci.ContractUnitArea))
                 .ForMember(c => c.EstateArea, opt => opt.MapFrom(nci =>nci.ContractEstateArea))
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<NewContractInput, Property>()
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<NewContractInput, Supply>()
                .IgnoreUnmappedProperties();
        }
    }
}
