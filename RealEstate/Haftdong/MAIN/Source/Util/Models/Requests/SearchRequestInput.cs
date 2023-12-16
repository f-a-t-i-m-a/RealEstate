using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Requests
{
    [TsClass]
    public class SearchRequestInput
    {
        public IntentionOfCustomer? IntentionOfCustomer { get; set; }
        public RequestState? State { get; set; }
        public PropertyType? PropertyType { get; set; }
        public UsageType? UsageType { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsPublic { get; set; }
        public Vicinity Vicinity { get; set; }
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

    }
}
