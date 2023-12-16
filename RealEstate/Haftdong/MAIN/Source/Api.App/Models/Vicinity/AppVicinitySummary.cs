using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Vicinity
{
    [TsClass]
    [AutoMapperConfig]
    public class AppVicinitySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string Name { get; set; }
        public string CompleteName { get; set; }

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }
        public LatLng CenterPoint { get; set; }
        public ObjectId? ParentID { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Vicinities.Vicinity, AppVicinitySummary>()
                .ForMember(p => p.CenterPoint, opt => opt.MapFrom(pp => pp.CenterPoint != null ? new LatLng
                {
                    Lat = pp.CenterPoint.Y,
                    Lng = pp.CenterPoint.X
                } : null))
//                .ForMember(s => s.Bounds, opt => opt.MapFrom(v =>GeographyUtils.FindBoundingBox(v.Boundary)))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<VicinitySummary, AppVicinitySummary>()
                 .ForMember(p => p.CenterPoint, opt => opt.MapFrom(pp => pp.CenterPoint != null ? new LatLng
                 {
                     Lat = pp.CenterPoint.Y,
                     Lng = pp.CenterPoint.X
                 } : null))
                //                .ForMember(s => s.Bounds, opt => opt.MapFrom(v => GeographyUtils.FindBoundingBox(v.Boundary)))
                .IgnoreUnmappedProperties();
        }
    }
}