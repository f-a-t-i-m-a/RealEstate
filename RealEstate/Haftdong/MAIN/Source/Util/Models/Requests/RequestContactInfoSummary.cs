using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    [AutoMapperConfig]
    public class RequestContactInfoSummary
    {
        public ObjectId RequestID { get; set; }
        public CustomerSummary Owner { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public ContactMethodCollectionSummary OwnerContact { get; set; }
        public ContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Request, RequestContactInfoSummary>()
                .ForMember(r => r.RequestID, opt => opt.MapFrom(re => re.ID))
                .IgnoreUnmappedProperties();
        }
    }
}