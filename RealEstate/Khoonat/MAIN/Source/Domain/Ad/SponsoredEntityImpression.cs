using System;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Ad
{
    public class SponsoredEntityImpression : ICreationTime
    {
        public long ID { get; set; }
        public Guid GUID { get; set; }
        public DateTime CreationTime { get; set; }

        public decimal BidAmount { get; set; }

        public long SponsoredEntityID { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }

        public long? ContentOwnerUserID { get; set; }
        public User ContentOwnerUser { get; set; }

        public long HttpSessionID { get; set; }
        public HttpSession HttpSession { get; set; }

        public long? BillingEntityID { get; set; }
        public SponsoredEntityImpressionBilling BillingEntity { get; set; }
    }
}