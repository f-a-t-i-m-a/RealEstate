using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSupplySummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public SalePriceSpecificationType? PriceSpecificationType { get; set; }
        public decimal? PropertyTotalPrice { get; set; }
        public decimal? PricePerEstateArea { get; set; }
        public decimal? PricePerUnitArea { get; set; }

        public decimal? PropertyMortgage { get; set; }
        public decimal? PropertyRent { get; set; }

//        public bool? OwnerCanBeContacted { get; set; }
//        public ContactMethodCollectionSummary OwnerContact { get; set; }
//        public ContactMethodCollectionSummary AgencyContact { get; set; }
        public AppPropertyDetails PropertyDetail { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Supply.Supply, AppSupplySummary>()
                .ForMember(s => s.PropertyTotalPrice, opt => opt.MapFrom(ss => ss.TotalPrice))
                .ForMember(s => s.PropertyMortgage, opt => opt.MapFrom(ss => ss.Mortgage))
                .ForMember(s => s.PropertyRent, opt => opt.MapFrom(ss => ss.Rent))
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<SupplyReference, AppSupplySummary>()
                .ForMember(s => s.PropertyTotalPrice, opt => opt.MapFrom(ss => ss.TotalPrice))
                .ForMember(s => s.PropertyMortgage, opt => opt.MapFrom(ss => ss.Mortgage))
                .ForMember(s => s.PropertyRent, opt => opt.MapFrom(ss => ss.Rent))
              .IgnoreUnmappedProperties();

            Mapper.CreateMap<SupplySummary, AppSupplySummary>()
                .ForMember(s => s.PropertyDetail, opt => opt.MapFrom(ss => ss.Property))
                .ForMember(s => s.PropertyTotalPrice, opt => opt.MapFrom(ss => ss.TotalPrice))
                .ForMember(s => s.PropertyMortgage, opt => opt.MapFrom(ss => ss.Mortgage))
                .ForMember(s => s.PropertyRent, opt => opt.MapFrom(ss => ss.Rent))
              .IgnoreUnmappedProperties();
        }
    }
}