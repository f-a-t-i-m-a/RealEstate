using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Contract;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Contracts
{
    [TsClass]
    [AutoMapperConfig]

    public class UpdateContractInput
    {
        public ObjectId ID { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime DeliveryDate { get; set; }
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

        public DateTime? LastIndexingTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? LastModifiedTimeByID { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? DeletedByID { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateContractInput, Contract>()
                .ForMember(c => c.TotalPrice, opt => opt.MapFrom(nci => (nci.ContractTotalPrice ?? null)))
                 .ForMember(c => c.Mortgage, opt => opt.MapFrom(nci => (nci.ContractMortgage ?? null)))
                 .ForMember(c => c.Rent, opt => opt.MapFrom(nci => (nci.ContractRent ?? null)))
                 .ForMember(c => c.UnitArea, opt => opt.MapFrom(nci => (nci.ContractUnitArea ?? null)))
                 .ForMember(c => c.EstateArea, opt => opt.MapFrom(nci => (nci.ContractEstateArea ?? null)))
                .IgnoreUnmappedProperties();
        }
    }
}
