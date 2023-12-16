using System;
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
    public class ApplicationUserDetails : ApplicationUserSummary
    {
        public ObjectId CreatedByID { get; set; }
        public DateTime? DeletionTime { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? LastLoginAttempt { get; set; }
        public int FailedLoginAttempts { get; set; }
        
        public string About { get; set; }
        public string WebSiteUrl { get; set; }

        public ContactMethodCollectionSummary Contact { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ApplicationUser, ApplicationUserDetails>()
                .IncludeBase<ApplicationUser, ApplicationUserSummary>()
                .IgnoreUnmappedProperties();
        }

    }
}