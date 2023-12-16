using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Property
{
    [TsClass]
    [AutoMapperConfig]
    public class PropertyReference
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime CreationTime { get; set; }
        public PropertyType PropertyType { get; set; }
        public CustomerReference Owner { get; set; }
        public PropertyState State { get; set; }
        public string LicencePlate { get; set; }
        public decimal? EstateArea { get; set; }
        public UsageType? UsageType { get; set; }
        public string Address { get; set; }
        public VicinityReference Vicinity { get; set; }
        public GeoJson2DCoordinates GeographicLocation { get; set; }
        public decimal? UnitArea { get; set; }
        public bool IsArchived { get; set; }
        public bool? IsHidden { get; set; }
        public bool? ConversionWarning { get; set; }
        public int? NumberOfRooms { get; set; }
        public ObjectId? CoverImageID { get; set; }
        public SourceType SourceType { get; set; }

        public List<SupplyReference> Supplies { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Property, PropertyReference>()
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<PropertyReference, Property>()
                .IgnoreUnmappedProperties();
        }
    }
}