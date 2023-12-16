using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewContactInfoInput
    {
        public string Value { get; set; }
        public ContactMethodType Type { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewContactInfoInput, NewContactInfoInput>()
                .IgnoreUnmappedProperties();
        }
    }
}