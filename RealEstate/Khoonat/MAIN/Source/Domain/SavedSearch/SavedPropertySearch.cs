using System;
using System.Collections.Generic;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.SavedSearch
{
	public class SavedPropertySearch : ICreationTime
	{
		public long ID { get; set; }
		public string Title { get; set; }

		public User User { get; set; }
		public long UserID { get; set; }

		public long? CreatorSessionID { get; set; }
		public HttpSession CreatorSession { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime? DeleteTime { get; set; }

		//
		// Search criteria

		public PropertyType? PropertyType { get; set; }
		public IntentionOfOwner? IntentionOfOwner { get; set; }

		public ICollection<SavedPropertySearchGeographicRegion> GeographicRegions { get; set; }

		public string AdditionalFilters { get; set; }

		//
		// Notification criteria

		public bool SendNotificationEmails { get; set; }
		public bool SendPromotionalSmsMessages { get; set; }
		public bool SendPaidSmsMessages { get; set; }
		public DateTime? SendNotificationsUntil { get; set; }

		public long NumberOfNotificationEmailsSent { get; set; }
		public long NumberOfPromotionalSmsMessagesSent { get; set; }
		public long NumberOfPaidSmsMessagesSent { get; set; }

		public UserContactMethod EmailNotificationTarget { get; set; }
		public long? EmailNotificationTargetID { get; set; }

		public UserContactMethod SmsNotificationTarget { get; set; }
		public long? SmsNotificationTargetID { get; set; }

		public SavedPropertySearchEmailNotificationType? EmailNotificationType { get; set; }
		public SavedPropertySearchSmsNotificationPart? SmsNotificationParts { get; set; }
	}
}