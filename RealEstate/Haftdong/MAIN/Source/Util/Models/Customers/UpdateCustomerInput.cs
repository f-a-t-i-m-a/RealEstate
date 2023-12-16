using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Customers
{
    [TsClass]
    [AutoMapperConfig]

    public class UpdateCustomerInput
    {
        public ObjectId ID { get; set; }
        public string DisplayName { get; set; }
        public int Age { get; set; }
        public NewContactMethodCollectionInput Contact { get; set; }
    //        public List<NewContactInfoInput> ContactInfos { get; set; }
    public string Description { get; set; }
        public int RequestCount { get; set; }
        public int PropertyCount { get; set; }
        public bool? IsMarried { get; set; }

        public string NameOfFather { get; set; }
        public int? Identification { get; set; }
        public string IssuedIn { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public CustomerReference Deputy { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateCustomerInput, Customer>()
                .IgnoreUnmappedProperties();
        }
    }
}
