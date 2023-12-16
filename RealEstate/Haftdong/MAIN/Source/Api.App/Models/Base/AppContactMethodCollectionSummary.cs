using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class AppContactMethodCollectionSummary
    {
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public List<AppPhoneInfoSummary> Phones { get; set; }
        public List<AppEmailInfoSummary> Emails { get; set; }
        public List<AppAddressInfoSummary> Addresses { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ContactMethodCollectionSummary, AppContactMethodCollectionSummary>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<ContactMethodCollection, AppContactMethodCollectionSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}