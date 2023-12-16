using System;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
    public class SavedPropertySearchPromotionalSmsNotDeliveredReturn : IBillingSourceEntity
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }

        public long PropertyListingID { get; set; }
        public PropertyListing PropertyListing { get; set; }

        public SavedPropertySearchPromotionalSms PromotionalSms { get; set; }
        public long PromotionalSmsID { get; set; }

        #region IBillingSourceEntity implementation

        public BillingSourceEntityState BillingState { get; set; }
        public User TargetUser { get; set; }
        public long? TargetUserID { get; set; }
        public UserBillingTransaction ForwardTransaction { get; set; }
        public long? ForwardTransactionID { get; set; }
        public UserBillingTransaction ReverseTransaction { get; set; }
        public long? ReverseTransactionID { get; set; }

        #endregion
    }
}