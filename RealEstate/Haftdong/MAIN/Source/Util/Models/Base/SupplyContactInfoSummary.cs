using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Supply;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Base
{
    [TsClass]
    [AutoMapperConfig]
    public class SupplyContactInfoSummary
    {
        public ObjectId SupplyID { get; set; }
        public bool OwnerCanBeContacted { get; set; }
        public ContactMethodCollectionSummary OwnerContact { get; set; }
        public ContactMethodCollectionSummary AgencyContact { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Supply, SupplyContactInfoSummary>()
                .ForMember(s => s.SupplyID, opt => opt.MapFrom(su => su.ID))
                .IgnoreUnmappedProperties();
        }
    }
}