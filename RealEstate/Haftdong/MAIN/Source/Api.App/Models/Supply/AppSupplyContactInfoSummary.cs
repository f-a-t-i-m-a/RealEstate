using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSupplyContactInfoSummary
    {
        public ObjectId SupplyID { get; set; }
        public bool OwnerCanBeContacted { get; set; }
        public AppContactMethodCollectionSummary OwnerContact { get; set; }
        public AppContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<SupplyContactInfoSummary, AppSupplyContactInfoSummary>()
                .IgnoreUnmappedProperties();
        }
    }
}