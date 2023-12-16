using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Customer
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSearchCustomerInput
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public CustomerSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppSearchCustomerInput, SearchCustomerInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
