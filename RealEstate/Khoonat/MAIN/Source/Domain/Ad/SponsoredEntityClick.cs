using System;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Ad
{
    public class SponsoredEntityClick : ICreationTime
    {
        public long ID { get; set; }
        public DateTime CreationTime { get; set; }

        public long SponsoredEntityID { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }

        public long ImpressionID { get; set; }
        public SponsoredEntityImpression Impression { get; set; }

        public long HttpSessionID { get; set; }
        public HttpSession HttpSession { get; set; }

        public long? BillingEntityID { get; set; }
        public SponsoredEntityClickBilling BillingEntity { get; set; }
    }
}