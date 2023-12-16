using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class UpdateApplicationUserInput
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public string WebSiteUrl { get; set; }
        public UserType Type { get; set; }
        public LicenseType? LicenseType { get; set; }
        public DateTime? LicenseActivationTime { get; set; }

        public List<NewContactInfoInput> ContactInfos { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateApplicationUserInput, ApplicationUser>()
                .IgnoreUnmappedProperties();
        }
    }
}
