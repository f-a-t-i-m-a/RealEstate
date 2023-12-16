using System;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
    public class SavedPropertySearchEmailNotification
    {
        public long ID { get; set; }
        public DateTime CreationDate { get; set; }
        
        public long PropertyListingID { get; set; }
        public PropertyListing PropertyListing { get; set; }

        public long SavedSearchID { get; set; }
        public SavedPropertySearch SavedSearch { get; set; }

        public long ContactMethodID { get; set; }
        public UserContactMethod ContactMethod { get; set; }
    }
}