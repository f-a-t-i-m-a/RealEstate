using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    [AutoMapperConfig]
    public class SupplySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public SupplyState State { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool? IsArchived { get; set; }
        public bool IsPublic { get; set; }

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }

        public PropertySummary Property { get; set; }

        public ObjectId CreatedByID { get; set; }
        public string CreatorFullName { get; set; }

        public DateTime? LastFetchTime { get; set; }

        public bool? ContactToOwner { get; set; }
        public bool? ContactToAgency { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Supply, SupplySummary>()
                .ForMember(s => s.ContactToOwner, opt => opt.MapFrom(su => su.OwnerCanBeContacted == true))
                .ForMember(s => s.ContactToAgency, opt => opt.MapFrom(su => su.OwnerCanBeContacted == false))
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<SupplyReference, SupplySummary>()
              .IgnoreUnmappedProperties();
        }
    }
}