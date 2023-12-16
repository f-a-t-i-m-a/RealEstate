using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Domain.Users
{
    [ElasticsearchType(Name = Types.UserType)]
    public class ApplicationUserIE
    {
        [String(Index = FieldIndexOption.No, Store = false)]
        public string ID { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string UserName { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long CreationTime { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long DeletionTime { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string DisplayName { get; set; }


        [String(Index = FieldIndexOption.NotAnalyzed, Store = false)]
        public string Type { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string ContactValues { get; set; }

        [Number(Index = NonStringIndexOption.NotAnalyzed, Store = true)]
        public long LicenseActivationTime { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = false)]
        public string LicenseType { get; set; }

        [String(Index = FieldIndexOption.NotAnalyzed, Store = true)]
        public string CreatedByID { get; set; }
    }
}