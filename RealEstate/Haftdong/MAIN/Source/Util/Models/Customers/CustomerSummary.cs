using System;
using System.Linq;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Customers
{
    [TsClass]
    [AutoMapperConfig]
    public class CustomerSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string DisplayName { get; set; }

        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public int RequestCount { get; set; }
        public int PropertyCount { get; set; }
        public ObjectId CreatedByID { get; set; }
        public string CreatorFullName { get; set; }

        public DateTime? LastVisitTime { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Customer, CustomerSummary>()
                .ForMember(c => c.PhoneNumber,
                    opt =>
                        opt.MapFrom(
                            cu =>
                                cu.Contact != null && cu.Contact.Phones != null && cu.Contact.Phones.Count != 0
                                    ? cu.Contact.Phones.First().NormalizedValue
                                    : ""))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<CustomerReference, CustomerSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}