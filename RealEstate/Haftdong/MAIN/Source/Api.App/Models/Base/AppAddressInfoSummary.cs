using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class AppAddressInfoSummary
    {
        public ObjectId ID { get; set; }
        public string NormalizedValue { get; set; }

        public bool IsVerifiable { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AddressInfoSummary, AppAddressInfoSummary>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<AddressInfo, AppAddressInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}