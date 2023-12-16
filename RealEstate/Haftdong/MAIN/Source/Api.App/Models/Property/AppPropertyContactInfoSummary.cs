using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Api.App.Models.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class AppPropertyContactInfoSummary
    {
        public List<AppSupplyContactInfoSummary> ContactInfos { get; set; }
        public AppCustomerSummary Owner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<PropertyContactInfoSummary, AppPropertyContactInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}