using System.Collections.Generic;
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
    public class NewVicinityInput
    {
        public string Name { get; set; }
        public VicinityType? Type { get; set; }

        public bool ShowTypeInTitle { get; set; }
        public string AlternativeNames { get; set; }
        public string AdditionalSearchText { get; set; }

        public VicinityType? WellKnownScope { get; set; }

        public bool ShowInSummary { get; set; }
        public bool CanContainPropertyRecords { get; set; }

        public ObjectId? CurrentVicinityID { get; set; }
        public IEnumerable<VicinityType> AllowedVicinityTypes { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<NewVicinityInput, Vicinity>()
                .IgnoreUnmappedProperties();
        }
    }
}
