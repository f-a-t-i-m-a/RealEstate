using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class AppRequestSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public UsageType? UsageType { get; set; }
        public long[] PropertyTypes { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public List<string> Vicinities { get; set; }
        public bool IsPublic { get; set; }

        #region Sales price
        public decimal? TotalPrice { get; set; }
        #endregion

        #region Rent price
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        #endregion

//        public CustomerReference Owner { get; set; }
//        public bool? OwnerCanBeContacted { get; set; }
//        public ContactMethodCollectionSummary OwnerContact { get; set; }
//        public ContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Domain.Request.Request, AppRequestSummary>()
                .ForMember(r => r.PropertyTypes, opt => opt.MapFrom(rr => rr.PropertyTypes))
                .Ignore(r => r.Vicinities)
                .IgnoreUnmappedProperties();

            Mapper.CreateMap<RequestSummary, AppRequestSummary>()
                .IgnoreUnmappedProperties();
            Mapper.CreateMap<RequestDetails, AppRequestSummary>()
               .IgnoreUnmappedProperties();
        }
    }
}