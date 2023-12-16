using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewSecretForUserApplicationInput
    {
        public string UserID { get; set; }
        public string ContactMethodID { get; set; }
        public ContactMethodType ContactMethodType { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewSecretForUserApplicationInput, NewSecretForUserApplicationInput>()
                .IgnoreUnmappedProperties();
        }
    }
   
}
