using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Property;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    [TsClass]
    [AutoMapperConfig]
    public class ApplicationUserReference
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<ApplicationUser, ApplicationUserReference>()
                .IgnoreUnmappedProperties();
        }
    }
}