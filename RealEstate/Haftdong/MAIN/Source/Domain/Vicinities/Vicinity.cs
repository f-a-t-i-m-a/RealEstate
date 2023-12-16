using System;
using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Nest;

namespace JahanJooy.RealEstateAgency.Domain.Vicinities
{
    public class Vicinity
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public long SqlID { get; set; }

        public string Name { get; set; }
        public string AlternativeNames { get; set; }
        public string AdditionalSearchText { get; set; }
        public string Description { get; set; }
        public string OfficialLinkUrl { get; set; }
        public string WikiLinkUrl { get; set; }
        public string AdministrativeNotes { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }
        public bool ShowTypeInTitle { get; set; }
        public bool ShowInHierarchy { get; set; }
        public bool ShowInSummary { get; set; }

        public bool CanContainPropertyRecords { get; set; }

        public GeoJson2DCoordinates CenterPoint { get; set; }
        public GeoJsonPolygon<GeoJson2DCoordinates> Boundary { get; set; }
        public double? BoundaryArea { get; set; }

        [BsonIgnore]
        public Vicinity Parent { get; set; }
        public ObjectId? ParentID { get; set; }
        public long? SqlParentID { get; set; }
        [BsonIgnore]
        public List<Vicinity> Children { get; set; }
        
        public Vicinity()
        {
        }

        public Vicinity(Vicinity source)
        {
            ID = source.ID;
            Name = source.Name;
            AlternativeNames = source.AlternativeNames;
            AdditionalSearchText = source.AdditionalSearchText;
            Description = source.Description;
            AdministrativeNotes = source.AdministrativeNotes;
            Enabled = source.Enabled;
            Order = source.Order;

            Type = source.Type;
            WellKnownScope = source.WellKnownScope;
            ShowTypeInTitle = source.ShowTypeInTitle;
            ShowInSummary = source.ShowInSummary;

            CanContainPropertyRecords = source.CanContainPropertyRecords;

            CenterPoint = source.CenterPoint;
            Boundary = source.Boundary;
            BoundaryArea = source.BoundaryArea;

            ParentID = source.ParentID;
        }
        public static Vicinity Copy(Vicinity source)
        {
            return source == null ? null : new Vicinity(source);
        }

        public static List<Vicinity> Copy(List<Vicinity> source)
        {
            return source?.Select(Copy).ToList();
        }

        public IEnumerable<Vicinity> GetParents()
        {
            return GetParentsInclusive().Skip(1);
        }

        public IEnumerable<Vicinity> GetParentsInclusive()
        {
            var vicinity = this;

            while (vicinity != null)
            {
                if (vicinity.ParentID.HasValue && vicinity.Parent == null)
                    throw new InvalidOperationException("Parent property" +
                                                        " is not populated. This method should be called on the Vicinity entities that have their Parent property loaded / populated.");

                yield return vicinity;
                vicinity = vicinity.Parent;
            }
        }
    }
}