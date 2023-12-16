using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem
{
    [AutoMapperConfig]
    public class ConfigureDataItemSummary
    {
        public ObjectId? ID { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ConfigurationDataItem, ConfigureDataItemSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}
