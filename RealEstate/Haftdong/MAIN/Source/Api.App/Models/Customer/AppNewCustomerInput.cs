using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [TsClass]
    [AutoMapperConfig]
    public class AppNewCustomerInput
    {
        public ObjectId ID { get; set; }
        public string DisplayName { get; set; }
        public List<NewContactInfoInput> ContactInfos { get; set; }
        public string Description { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppNewCustomerInput, Domain.Customers.Customer>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<AppNewCustomerInput, CustomerReference>()
               .IgnoreUnmappedProperties();

            Mapper.CreateMap<AppNewCustomerInput, NewCustomerInput>()
                .IgnoreUnmappedProperties();
           
        }
    }
}
