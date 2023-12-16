using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class SignupInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public string WebSiteUrl { get; set; }
        public ObjectId? ProfilePictureID { get; set; }

        public List<NewContactInfoInput> ContactInfos { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<SignupInput, ApplicationUser>()
                .IgnoreUnmappedProperties();
        }
    }
}
