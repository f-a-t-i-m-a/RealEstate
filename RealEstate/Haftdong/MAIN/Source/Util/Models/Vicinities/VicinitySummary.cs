using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    [AutoMapperConfig]
    public class VicinitySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }

        public string Name { get; set; }
        public string AlternativeNames { get; set; }
        public string AdditionalSearchText { get; set; }
        public string Description { get; set; }
        public string OfficialLinkUrl { get; set; }
        public string WikiLinkUrl { get; set; }
        public string AdministrativeNotes { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }

        public string CompleteName { get; set; }

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }
        public bool ShowTypeInTitle { get; set; }
        public bool ShowInHierarchy { get; set; }
        public bool ShowInSummary { get; set; }

        public bool CanContainPropertyRecords { get; set; }
        public GeoJson2DCoordinates CenterPoint { get; set; }
        public GeoJsonPolygon<GeoJson2DCoordinates> Boundary { get; set; }

        // Self-referencing hierarchy relationship
        public VicinitySummary Parent { get; set; }
        public ObjectId? ParentID { get; set; }
        public List<VicinitySummary> Children { get; set; }


        // Collections of entities related to Vicinity
        public List<PropertyReference> Properties { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Vicinity, VicinitySummary>()
                .IgnoreUnmappedProperties();
        }
    }
}