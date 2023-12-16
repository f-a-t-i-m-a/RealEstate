using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Customers;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [AutoMapperConfig]
    public class AppCustomerDetails : AppCustomerSummary
    {
        public int? Age { get; set; }
        public bool? IsMarried { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? DeletionTime { get; set; }
      
        public string NameOfFather { get; set; }
        public int? Identification { get; set; }
        public string IssuedIn { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public CustomerReference Deputy { get; set; }

        public new static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<CustomerDetails, AppCustomerDetails>()
                .IgnoreUnmappedProperties();
        }
    }
}