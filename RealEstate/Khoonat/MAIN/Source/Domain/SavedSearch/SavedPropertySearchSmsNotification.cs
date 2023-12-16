using System;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
    public class SavedPropertySearchSmsNotification
    {
        public long ID { get; set; }
        public DateTime CreationTime { get; set; }

        public bool? Delivered { get; set; }
        public int NumberOfSmsSegments { get; set; }

        public long PropertyListingID { get; set; }
        public PropertyListing PropertyListing { get; set; }

        public long SavedSearchID { get; set; }
        public SavedPropertySearch SavedSearch { get; set; }

        public long ContactMethodID { get; set; }
        public UserContactMethod ContactMethod { get; set; }

        public long TargetUserID { get; set; }
        public User TargetUser { get; set; }

        public long? BillingID { get; set; }
        public SavedSearchSmsNotificationBilling Billing { get; set; }
    }
}