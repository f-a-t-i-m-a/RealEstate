using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    [AutoMapperConfig]
    public class UpdateVicinityInput
    {
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

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }
        public bool ShowTypeInTitle { get; set; }
        public bool ShowInHierarchy { get; set; }
        public bool ShowInSummary { get; set; }

        public bool CanContainPropertyRecords { get; set; }

//        public DbGeography CenterPoint { get; set; }
//        public DbGeography Boundary { get; set; }


        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<UpdateVicinityInput, Vicinity>()
                .IgnoreUnmappedProperties();
        }
    }
}
