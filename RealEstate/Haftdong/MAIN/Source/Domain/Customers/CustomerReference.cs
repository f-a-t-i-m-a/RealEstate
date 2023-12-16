using System;
using System.Linq;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Customers
{
    [TsClass]
    [AutoMapperConfig]
    public class CustomerReference
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string NameOfFather { get; set; }
        public int? Identification { get; set; }
        public string IssuedIn { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Customer, CustomerReference>()
                .ForMember(c => c.PhoneNumber,
                    opt =>
                        opt.MapFrom(
                            cu => cu.Contact != null && cu.Contact.Phones != null && cu.Contact.Phones.Count != 0
                                ? cu.Contact.Phones.First().NormalizedValue
                                : ""))
                .ForMember(c => c.Email,
                    opt =>
                        opt.MapFrom(
                            cu => cu.Contact != null && cu.Contact.Emails != null && cu.Contact.Emails.Count != 0
                                ? cu.Contact.Emails.First().NormalizedValue
                                : ""))
                .ForMember(c => c.Address,
                    opt =>
                        opt.MapFrom(
                            cu => cu.Contact != null && cu.Contact.Addresses != null && cu.Contact.Addresses.Count != 0
                                ? cu.Contact.Addresses.First().NormalizedValue
                                : ""))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<CustomerReference, Customer>()
                .IgnoreUnmappedProperties();
        }
    }
}