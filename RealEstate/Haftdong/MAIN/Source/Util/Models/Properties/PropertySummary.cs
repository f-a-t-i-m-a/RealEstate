using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Properties
{
    [TsClass]
    [AutoMapperConfig]
    public class PropertySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public PropertyType PropertyType { get; set; }
        public PropertyState State { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreationTime { get; set; }
        public ObjectId? CreatedByID { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public ObjectId? CoverImageID { get; set; }

        public string Address { get; set; }
        public VicinityReference Vicinity { get; set; }
        public GeoJson2DCoordinates GeographicLocation { get; set; }
        public decimal? EstateArea { get; set; }
        public decimal? UnitArea { get; set; }
        public UsageType? UsageType { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime? LastFetchTime { get; set; }

        public bool? IsHidden { get; set; }
        public bool? ConversionWarning { get; set; }
        public int? NumberOfRooms { get; set; }

        public List<SupplySummary> Supplies { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Property, PropertySummary>()
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<PropertyReference, PropertySummary>()
            .IgnoreUnmappedProperties();
        }
    }
}