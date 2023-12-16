using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Contract
{
    [ElasticsearchType(Name = Types.ContractType)]
    public class ContractIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long ContractDate { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Description { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? TotalPrice { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Rent { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public decimal? Mortgage { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public int? TrackingID { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }
    }
}