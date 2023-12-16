using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSignupInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        public List<NewContactInfoInput> ContactInfos { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppSignupInput, ApplicationUser>()
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<AppSignupInput, SignupInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
