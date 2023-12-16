using System;
using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    [AutoMapperConfig]
    public class RequestSummary
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }
        public UsageType? UsageType { get; set; }
        public RequestState State { get; set; }
        public SourceType SourceType { get; set; }
        public long[] PropertyTypes { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }


        #region Sales price
        public decimal? TotalPrice { get; set; }
        #endregion

        #region Rent price
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }
        #endregion
        
        public ObjectId CreatedByID { get; set; }
        public string CreatorFullName { get; set; }
        public List<string> Vicinities { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Request, RequestSummary>()
                .ForMember(rs => rs.CreatedByID, opt => opt.MapFrom(r => r.CreatedByID))
                .Ignore(r => r.Vicinities)
                .IgnoreUnmappedProperties();

        }
    }
}