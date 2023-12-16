using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Contract
{
    [TsClass]
    [AutoMapperConfig]
    public class ContractReference
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public DateTime ContractDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Mortgage { get; set; }
        public decimal? Rent { get; set; }

        public int? TrackingID { get; set; }

        public string PropertyOwner { get; set; }
        public string RequestOwner { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Contract, ContractReference>()
                .IgnoreUnmappedProperties();
        }
    }
}
