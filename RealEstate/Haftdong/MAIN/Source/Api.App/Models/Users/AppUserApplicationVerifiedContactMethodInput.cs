using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class AppUserApplicationVerifyContactMethodInput
    {
        public string ContactMethodID { get; set; }
        public string VerificationSecret { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppUserApplicationVerifyContactMethodInput, UserApplicationVerifyContactMethodInput>()
                .IgnoreUnmappedProperties();
        }
    }
   
}
