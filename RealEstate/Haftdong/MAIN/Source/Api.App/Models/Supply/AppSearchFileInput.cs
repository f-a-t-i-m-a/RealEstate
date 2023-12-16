using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Supply
{
    [TsClass]
    [AutoMapperConfig]
    public class AppSearchFileInput
    {
        public UsageType? UsageType { get; set; }
        public PropertyType? PropertyType { get; set; }
        public IntentionOfOwner? IntentionOfOwner { get; set; }
        public SourceType? SourceType { get; set; }
        public bool? IsPublic { get; set; }
        public bool? MineOnly { get; set; }
        public decimal? EstateAreaMin { get; set; }
        public decimal? EstateAreaMax { get; set; }
        public decimal? UnitAreaMin { get; set; }
        public decimal? UnitAreaMax { get; set; }
        public int? NumberOfRoomsMin { get; set; }
        public int? NumberOfRoomsMax { get; set; }
        public decimal? MortgageMin { get; set; }
        public decimal? MortgageMax { get; set; }
        public decimal? RentMin { get; set; }
        public decimal? RentMax { get; set; }
        public decimal? PricePerEstateAreaMin { get; set; }
        public decimal? PricePerEstateAreaMax { get; set; }
        public decimal? PricePerUnitAreaMin { get; set; }
        public decimal? PricePerUnitAreaMax { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public Domain.Vicinities.Vicinity Vicinity { get; set; }
        public ObjectId? VicinityID { get; set; }
        public LatLngBounds Bounds { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public SupplySortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppSearchFileInput, SearchFileInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
