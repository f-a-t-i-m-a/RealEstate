using System;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Base
{
    [ElasticsearchType(Name = Types.SearchType)]
    public class SearchIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string DataType { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = true)]
        public string IndexedText { get; set; }

        [String(Index = FieldIndexOption.No, Store = true)]
        public string Title { get; set; }

        [String(Index = FieldIndexOption.No, Store = true)]
        public string DisplayText { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long CreationTime { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long? ModificationTime { get; set; }
    }
}