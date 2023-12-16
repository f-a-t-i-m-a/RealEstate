using System;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
    public class SavedPropertySearchPromotionalSms : IBillingSourceEntity
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }

        public long PropertyListingID { get; set; }
        public PropertyListing PropertyListing { get; set; }

        public string MessageText { get; set; }
        public int NumberOfSmsSegments { get; set; }

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