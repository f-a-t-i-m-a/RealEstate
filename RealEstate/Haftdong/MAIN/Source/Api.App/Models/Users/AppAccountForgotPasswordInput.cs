using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class AppAccountForgotPasswordInput
    {
        public string UserName { get; set; }
        public string VerificationPhoneNumber { get; set; }
        public string VerificationEmailAddress { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppAccountForgotPasswordInput, AccountForgotPasswordInput>()
                .IgnoreUnmappedProperties();
        }

    }
}
