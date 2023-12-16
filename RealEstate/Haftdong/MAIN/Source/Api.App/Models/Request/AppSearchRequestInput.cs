using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Request
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSearchRequestInput
    {
        public IntentionOfCustomer? IntentionOfCustomer { get; set; }
        public PropertyType? PropertyType { get; set; }
        public Domain.Vicinities.Vicinity Vicinity { get; set; }
        public ObjectId? VicinityID { get; set; }
        public decimal? EstateAreaMin { get; set; }
        public decimal? EstateAreaMax { get; set; }
        public decimal? UnitAreaMin { get; set; }
        public decimal? UnitAreaMax { get; set; }
        public decimal? MortgageMin { get; set; }
        public decimal? MortgageMax { get; set; }
        public decimal? RentMin { get; set; }
        public decimal? RentMax { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public RequestSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppSearchRequestInput, SearchRequestInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
