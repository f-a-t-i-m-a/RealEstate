using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class ApplicationUserSummary
    {
        [BsonId]
        public string Id { get; set; }
        public long Code { get; set; }
        public bool IsOperator { get; set; }

        public string UserName { get; set; }
        public bool IsEnabled { get; set; }

        public DateTime ModificationTime { get; set; }

        public LicenseType? LicenseType { get; set; }
        public DateTime? LicenseActivationTime { get; set; }

        public string DisplayName { get; set; }
        public UserType Type { get; set; }
        public PhotoInfoSummary ProfilePicture { get; set; }

        public bool IsVerified { get; set; }
        public List<string> Roles { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ApplicationUser, ApplicationUserSummary>()
                .IgnoreUnmappedProperties();
        }

    }
}