using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.App.Models.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class AppPropertySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public PropertyType PropertyType { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public ObjectId? CoverImageID { get; set; }

        public string Address { get; set; }
        public AppVicinityReference Vicinity { get; set; }
        public LatLng GeographicLocation { get; set; }
        public decimal? EstateArea { get; set; }
        public decimal? UnitArea { get; set; }
        public UsageType? UsageType { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime? LastFetchTime { get; set; }

//        public CustomerReference Owner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Property.Property, AppPropertySummary>()
                .ForMember(p => p.GeographicLocation, opt => opt.MapFrom(pp => pp.GeographicLocation != null ? new LatLng
                {
                    Lat = pp.GeographicLocation.Y,
                    Lng = pp.GeographicLocation.X
                } : null))
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<PropertyReference, AppPropertySummary>()
                .ForMember(p => p.GeographicLocation, opt => opt.MapFrom(pp => pp.GeographicLocation != null ? new LatLng
                {
                    Lat = pp.GeographicLocation.Y,
                    Lng = pp.GeographicLocation.X
                } : null))
            .IgnoreUnmappedProperties();
        }
    }
}