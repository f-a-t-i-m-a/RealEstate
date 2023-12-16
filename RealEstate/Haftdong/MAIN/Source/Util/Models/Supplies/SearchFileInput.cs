using System;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Supplies
{
    [TsClass]
    public class SearchFileInput
    {
        public UsageType? UsageType { get; set; }
        public PropertyType? PropertyType { get; set; }
        public IntentionOfOwner? IntentionOfOwner { get; set; }
        public SupplyState? State { get; set; }
        public SourceType? SourceType { get; set; }
        public bool? IsArchived { get; set; }
        public bool? HasPhoto { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsPublic { get; set; }
        public bool? HasWarning { get; set; }
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
        public Vicinity Vicinity { get; set; }
        public ObjectId? VicinityID { get; set; }
        public LatLngBounds Bounds { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public SupplySortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
