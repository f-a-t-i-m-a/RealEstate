using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem
{
    [TsClass]
    [AutoMapperConfig]
    public class UpdateInput
    {
        public ObjectId ID { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateInput, ConfigurationDataItem>()
                .IgnoreUnmappedProperties();
        }
    }
}
