using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class PhoneInfoSummary
    {
        public ObjectId ID { get; set; }

        public string Value { get; set; }
        public string NormalizedValue { get; set; }
        public bool CanReceiveSms { get; set; }

        public bool IsVerifiable { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<PhoneInfo, PhoneInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}