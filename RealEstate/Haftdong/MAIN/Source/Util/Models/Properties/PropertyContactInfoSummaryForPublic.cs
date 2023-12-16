using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    [AutoMapperConfig]
    public class PropertyContactInfoSummaryForPublic
    {
        public List<SupplyContactInfoSummary> ContactInfos { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<PropertyContactInfoSummary, PropertyContactInfoSummaryForPublic>()
                .IgnoreUnmappedProperties();
        }
    }
}