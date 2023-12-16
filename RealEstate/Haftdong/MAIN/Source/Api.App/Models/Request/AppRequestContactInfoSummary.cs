using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class AppRequestContactInfoSummary
    {
        public ObjectId RequestID { get; set; }
        public AppCustomerSummary Owner { get; set; }
        public bool? OwnerCanBeContacted { get; set; }
        public AppContactMethodCollectionSummary OwnerContact { get; set; }
        public AppContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<RequestContactInfoSummary, AppRequestContactInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}