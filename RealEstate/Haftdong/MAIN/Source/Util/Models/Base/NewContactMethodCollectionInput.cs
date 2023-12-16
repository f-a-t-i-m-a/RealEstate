using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class NewContactMethodCollectionInput
    {
        public string OrganizationName { get; set; }
        public string ContactName { get; set; }
        public List<NewContactInfoInput> Phones { get; set; }
        public List<NewContactInfoInput> Emails { get; set; }
        public List<NewContactInfoInput> Addresses { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewContactMethodCollectionInput, ContactMethodCollection>()
                .IgnoreUnmappedProperties();
        }
    }
}