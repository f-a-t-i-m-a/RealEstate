using System;
using System.Collections.Generic;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
    public class SavedSearchSmsNotificationBilling : IBillingSourceEntity
    {
        public long ID { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public DateTime NotificationsDate { get; set; }

        public int NumberOfNotifications { get; set; }
        public int NumberOfNotificationParts { get; set; }

        public ICollection<SavedPropertySearchSmsNotification> PropertyNotifications { get; set; }
        // For later: public ICollection<SavedRequestSearchSmsNotification> RequestNotifications { get; set; }

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