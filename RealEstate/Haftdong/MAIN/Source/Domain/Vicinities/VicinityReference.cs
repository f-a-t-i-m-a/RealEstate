using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Vicinities
{
    [TsClass]
    [AutoMapperConfig]
    public class VicinityReference
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string OfficialLinkUrl { get; set; }
        public string WikiLinkUrl { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }
        public int Region { get; set; }

        public string CompleteName { get; set; }

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }

        public bool CanContainPropertyRecords { get; set; }

        public GeoJson2DCoordinates CenterPoint { get; set; }
        public GeoJsonPolygon<GeoJson2DCoordinates> Boundary { get; set; }
        public ObjectId? ParentID { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Vicinity, VicinityReference>()
                .IgnoreUnmappedProperties();
        }
    }
}