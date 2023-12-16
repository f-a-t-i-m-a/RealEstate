using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class RequestReference
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public UsageType? UsageType { get; set; }
        public RequestState State { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPublic { get; set; }
        public SourceType SourceType { get; set; }

        #region Location properties

        public ObjectId[] Vicinities { get; set; }
        public long[] PropertyTypes { get; set; }
        public IntentionOfCustomer IntentionOfCustomer { get; set; }

        #endregion

        #region Rent price

        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }

        #endregion

        #region Sales price

        public decimal? TotalPrice { get; set; }

        #endregion

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Request, RequestReference>()
                .IgnoreUnmappedProperties();
        }
    }
}