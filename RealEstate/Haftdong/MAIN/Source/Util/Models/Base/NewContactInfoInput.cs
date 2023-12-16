using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class NewContactInfoInput
    {
        public string Value { get; set; }
        public ContactMethodType Type { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewContactInfoInput, PhoneInfo>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<NewContactInfoInput, EmailInfo>()
               .IgnoreUnmappedProperties();

            Mapper.CreateMap<NewContactInfoInput, AddressInfo>()
               .IgnoreUnmappedProperties();
        }
    }
}