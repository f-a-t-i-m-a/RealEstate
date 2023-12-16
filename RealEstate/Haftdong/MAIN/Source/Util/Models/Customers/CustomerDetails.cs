using System;
using System.Linq;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Base;

namespace JahanJooy.RealEstateAgency.Util.Models.Customers
{
    [AutoMapperConfig]
    public class CustomerDetails: CustomerSummary
    {
        public int? Age { get; set; }
        public bool? IsMarried { get; set; }
        public bool IsArchived { get; set; }
        public ContactMethodCollectionSummary Contact { get; set; }
        public DateTime? DeletionTime { get; set; }
      
        public string NameOfFather { get; set; }
        public int? Identification { get; set; }
        public string IssuedIn { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public CustomerReference Deputy { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Customer, CustomerDetails>()
                .IncludeBase<Customer, CustomerSummary>();

            Mapper.CreateMap<CustomerDetails, CustomerReference>()
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
        }
    }
}