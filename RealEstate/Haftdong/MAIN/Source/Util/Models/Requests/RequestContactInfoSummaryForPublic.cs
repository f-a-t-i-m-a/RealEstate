using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    [AutoMapperConfig]
    public class RequestContactInfoSummaryForPublic
    {
        public ObjectId RequestID { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public ContactMethodCollectionSummary OwnerContact { get; set; }
        public ContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<RequestContactInfoSummary, RequestContactInfoSummaryForPublic>()
                .IgnoreUnmappedProperties();
        }
    }
}