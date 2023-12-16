using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Supply
{
    [ElasticsearchType(Name = Types.SupplyType)]
    public class SupplyIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Description { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsArchived { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool HasPhoto { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? HasWarning { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsHidden { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsPublic { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? EstateArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? UnitArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? NumberOfRooms { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Mortgage { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Rent { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? TotalPrice { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? PricePerEstateArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? PricePerUnitArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long CreationTime { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long? ExpirationTime { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Vicinity { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string VicinityIds { get; set; }

        [GeoPoint(Store = true)]
        public LatLon GeographicLocation { get; set; }
    }
}