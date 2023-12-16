using MongoDB.Driver.GeoJsonObjectModel;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Property
{
    [ElasticsearchType(Name = Types.PropertyType)]
    public class PropertyIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Address { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Description { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsArchived { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? EstateArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? UnitArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? NumberOfRooms { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long CreationTime { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }

        [GeoPoint(Store = true)]
        public LatLon GeographicLocation { get; set; }
    }
}