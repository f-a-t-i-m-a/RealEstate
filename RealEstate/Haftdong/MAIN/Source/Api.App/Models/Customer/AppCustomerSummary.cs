using System.Linq;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [TsClass]
    [AutoMapperConfig]
    public class AppCustomerSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Customers.Customer, AppCustomerSummary>()
                .ForMember(c => c.PhoneNumber,
                    opt =>
                        opt.MapFrom(
                            cu =>
                                cu.Contact != null && cu.Contact.Phones != null && cu.Contact.Phones.Count != 0
                                    ? cu.Contact.Phones.First().NormalizedValue
                                    : ""))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<CustomerReference, AppCustomerSummary>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<CustomerSummary, AppCustomerSummary>()
                .IgnoreUnmappedProperties();
           
        }
     
    }
}
