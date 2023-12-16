using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem
{
    [TsClass]
    [AutoMapperConfig]
    public class AddNewInput
    {
        public string Identifier { get; set; }
        public string Value { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AddNewInput, ConfigurationDataItem>()
                .IgnoreUnmappedProperties();
        }
    }
}
