using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Customers
{
    [ElasticsearchType(
    Name = Types.CustomerType
    )]
    public class CustomerIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string DisplayName { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false )]
        public string Email { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Numbers { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public int RequestCount { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public int PropertyCount { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long LastVisitTime { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long DeletionTime { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }

        [Boolean(Index = NonStringIndexOption.NotAnalyzed, Store = false)]
        public bool? IsArchived { get; set; }
    }
}
