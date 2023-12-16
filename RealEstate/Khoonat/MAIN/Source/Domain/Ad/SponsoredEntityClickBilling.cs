using System;
using System.Collections.Generic;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Domain.Ad
{
    public class SponsoredEntityClickBilling : IBillingSourceEntity, ICreationTime
    {
        public long ID { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public DateTime ClicksDate { get; set; }
        public long SponsoredEntityID { get; set; }
        public SponsoredEntity SponsoredEntity { get; set; }

        public int NumberOfClicks { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<SponsoredEntityClick> Clicks { get; set; }

        #region IBillingSourceEntity implementation

        public User TargetUser { get; set; }
        public long? TargetUserID { get; set; }
        public BillingSourceEntityState BillingState { get; set; }
        public UserBillingTransaction ForwardTransaction { get; set; }
        public long? ForwardTransactionID { get; set; }
        public UserBillingTransaction ReverseTransaction { get; set; }
        public long? ReverseTransactionID { get; set; }

        #endregion
    }
}