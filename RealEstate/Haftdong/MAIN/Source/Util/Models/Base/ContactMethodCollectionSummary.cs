using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class ContactMethodCollectionSummary
    {
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public List<PhoneInfoSummary> Phones { get; set; }
        public List<EmailInfoSummary> Emails { get; set; }
        public List<AddressInfoSummary> Addresses { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ContactMethodCollection, ContactMethodCollectionSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}