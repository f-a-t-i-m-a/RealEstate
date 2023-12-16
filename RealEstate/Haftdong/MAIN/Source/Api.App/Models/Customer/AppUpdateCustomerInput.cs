using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [TsClass]
    [AutoMapperConfig]

    public class AppUpdateCustomerInput
    {
        public ObjectId ID { get; set; }
        public string DisplayName { get; set; }
        public List<NewContactInfoInput> ContactInfos { get; set; }
        public string Description { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppUpdateCustomerInput, Domain.Customers.Customer>()
                .IgnoreUnmappedProperties();
        }
    }
}
