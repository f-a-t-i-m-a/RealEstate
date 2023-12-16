using System.Collections.Generic;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Request
{
    [ElasticsearchType(Name = Types.RequestType)]
    public class RequestIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long CreationTime { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public List<long> PropertyTypes { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Mortgage { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Rent { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsArchived { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool IsPublic { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? TotalPrice { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? EstateArea { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? UnitArea { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Description { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long? ExpirationTime { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Vicinity { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string VicinityIds { get; set; }
    }
}