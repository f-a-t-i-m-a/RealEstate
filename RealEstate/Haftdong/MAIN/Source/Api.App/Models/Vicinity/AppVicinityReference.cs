using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Vicinity
{
    [TsClass]
    [AutoMapperConfig]
    public class AppVicinityReference
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

        public LatLng CenterPoint { get; set; }
        public ObjectId? ParentID { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<VicinityReference, AppVicinityReference>()
                .ForMember(p => p.CenterPoint, opt => opt.MapFrom(pp => pp.CenterPoint != null ? new LatLng
                {
                    Lat = pp.CenterPoint.Y,
                    Lng = pp.CenterPoint.X
                } : null))
                .IgnoreUnmappedProperties();
        }
    }
}