using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.EF;
using JahanJooy.Common.Util.Sms;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Services.Resources;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Core.Services.Enums;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Domain.SavedSearch;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Util.Presentation.Property;
using JahanJooy.RealEstate.Util.Resources;
using log4net;
using log4net.Util;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class SavedSearchService : ISavedSearchService
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (SavedSearchService));

	    private const int BillSavedSearchSmsNotificationsBatchSize = 50;
	    private const int FinalizePartialSavedSearchSmsNotificationBillingsBatchSize = 10;

		#region Injected dependencies

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IActivityLogService ActivityLogService { get; set; }

		[ComponentPlug]
		public IEmailNotificationService EmailNotificationService { get; set; }

		[ComponentPlug]
		public ISmsMessageService SmsMessageService { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public IUserBillingBalanceCache BalanceCache { get; set; }

        [ComponentPlug]
        public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }
        
        [ComponentPlug]
        public IUserBalanceService UserBalanceService { get; set; }

		#endregion

		#region Implementation of ISavedSearchService

		public ValidationResult SavePropertySearch(SavedPropertySearch savedPropertySearch)
		{
			if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.IsCrawler || !ServiceContext.Principal.CoreIdentity.IsAuthenticated)
				throw new InvalidOperationException("Not an authenticated user-interactive session.");

            if (!ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                throw new InvalidOperationException("The user is not specified in the context. Can't save a SavedPropertySearch anonymously.");

			var validationResult = ValidateSavedPropertySearch(savedPropertySearch);
			if (validationResult != null && !validationResult.IsValid)
				return validationResult;

			savedPropertySearch.CreatorSessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID));
			savedPropertySearch.UserID = ServiceContext.Principal.CoreIdentity.UserId.Value;
			OnUpdate(savedPropertySearch);

			DbManager.Db.SavedPropertySearchesDbSet.Add(savedPropertySearch);
			DbManager.SaveDefaultDbChanges();

			ActivityLogService.ReportActivity(TargetEntityType.SavedPropertySearch, savedPropertySearch.ID, ActivityAction.Create);
            RealEstateStaticLogs.SavedSearch.InfoFormat("SavedPropertySearch #{0} created.", savedPropertySearch.ID);

			return ValidationResult.Success;
		}

		public ValidationResult Update(SavedPropertySearch savedPropertySearch)
		{
			if (ServiceContext.CurrentSession.IsCrawler)
				throw new InvalidOperationException("Not a user-interactive session.");

            if (!ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                throw new InvalidOperationException("The user is not specified in the context. Can't save a SavedPropertySearch anonymously.");

            var validationResult = ValidateSavedPropertySearch(savedPropertySearch);
			if (!validationResult.IsValid)
				return validationResult;

			var dbSavedPropertySearch = DbManager.Db.SavedPropertySearchesDbSet.SingleOrDefault(sps => sps.ID == savedPropertySearch.ID);
			if (dbSavedPropertySearch == null)
				throw new ArgumentException("Saved search does not exist");

			if (dbSavedPropertySearch.DeleteTime.HasValue)
				throw new ArgumentException("Saved search is already deleted.");

			dbSavedPropertySearch.Title = savedPropertySearch.Title;

			dbSavedPropertySearch.SendNotificationEmails = savedPropertySearch.SendNotificationEmails;
			dbSavedPropertySearch.SendPromotionalSmsMessages = savedPropertySearch.SendPromotionalSmsMessages;
			dbSavedPropertySearch.SendPaidSmsMessages = savedPropertySearch.SendPaidSmsMessages;
			dbSavedPropertySearch.SendNotificationsUntil = savedPropertySearch.SendNotificationsUntil;
			dbSavedPropertySearch.EmailNotificationTargetID = savedPropertySearch.EmailNotificationTargetID;
			dbSavedPropertySearch.SmsNotificationTargetID = savedPropertySearch.SmsNotificationTargetID;
			dbSavedPropertySearch.EmailNotificationType = savedPropertySearch.EmailNotificationType;
			dbSavedPropertySearch.SmsNotificationParts = savedPropertySearch.SmsNotificationParts;

            DbManager.Db.ChangeTracker.DetectChanges();
            var changes = ExtractChanges(dbSavedPropertySearch);
            DbManager.Db.SavedPropertySearchUpdateHistoriesDSet.Add(changes);

			OnUpdate(dbSavedPropertySearch);

			ActivityLogService.ReportActivity(TargetEntityType.SavedPropertySearch, savedPropertySearch.ID, ActivityAction.Edit);
            RealEstateStaticLogs.SavedSearch.InfoFormat("SavedPropertySearch #{0} updated.", savedPropertySearch.ID);

			return ValidationResult.Success;
		}

		public void DeletePropertySearch(long id)
		{
			var dbSavedPropertySearch = DbManager.Db.SavedPropertySearchesDbSet.SingleOrDefault(sps => sps.ID == id);
			if (dbSavedPropertySearch == null)
				throw new ArgumentException("Saved search does not exist");

			if (dbSavedPropertySearch.DeleteTime.HasValue)
				throw new ArgumentException("Saved search is already deleted.");

			dbSavedPropertySearch.DeleteTime = DateTime.Now;
			ActivityLogService.ReportActivity(TargetEntityType.SavedPropertySearch, id, ActivityAction.Delete);
            RealEstateStaticLogs.SavedSearch.InfoFormat("SavedPropertySearch #{0} deleted.", id);
		}

		public int SendEmailToSavedPropertySearchTargets(long listingId)
		{
            Log.DebugFormat("Email for Listing {0} - Send emails service invoked.", listingId);

			var listing = DbManager.Db.PropertyListings.IncludeAllDetailsProperties().Single(l => l.ID == listingId);
			var listingDetails = PropertyListingDetails.MakeDetails(listing);
			var listingSummary = PropertyListingSummary.Summarize(listingDetails);

			var savedSearches = FilterQueryForMatchingPropertyListings(DbManager.Db.SavedPropertySearchesDbSet, listingDetails)
				.Where(sps => sps.SendNotificationEmails && sps.EmailNotificationTargetID.HasValue && sps.EmailNotificationTarget.IsVerified && !sps.EmailNotificationTarget.IsDeleted)
				.Include(sps => sps.User)
				.Include(sps => sps.EmailNotificationTarget)
				.ToList();

		    var matchingSearches = FindMatchingPropertySearchesForEmail(savedSearches, listing, listingDetails).ToList();

            if (matchingSearches.Count < 1)
                return 0;

			foreach (var matchingSearch in matchingSearches)
			{
                DbManager.Db.SavedPropertySearchEmailNotificationsDbSet.Add(new SavedPropertySearchEmailNotification
			    {
			        PropertyListingID = listingId,
			        SavedSearchID = matchingSearch.ID,
			        ContactMethodID = matchingSearch.EmailNotificationTargetID.GetValueOrDefault(),
			        CreationDate = DateTime.Now
			    });

				matchingSearch.NumberOfNotificationEmailsSent++;
				EmailNotificationService.SendPropertyInfoForSavedSearch(matchingSearch.User, listingDetails, listingSummary, matchingSearch);
            
                Log.DebugFormat("Email for Listing {0} - Saved search {1} matched, and contact method {2} received an email notification.", listingId, matchingSearch.ID, matchingSearch.EmailNotificationTargetID);
                RealEstateStaticLogs.SavedSearch.InfoFormat("Email: Listing {0}, Saved search {1} for user {2} contact method {3}, matched and notified.",
                    listingId, matchingSearch.ID, matchingSearch.UserID, matchingSearch.EmailNotificationTargetID);
            }

            Log.DebugFormat("Email for Listing {0} - Send emails service finished, {1} notifications sent.", listingId, matchingSearches.Count);
            return matchingSearches.Count;
		}

		public int SendSmsToSavedPropertySearchTargets(long listingId)
		{
            Log.DebugFormat("SMS for Listing {0} - Send sms service invoked.", listingId);

            var listing = DbManager.Db.PropertyListings.IncludeAllDetailsProperties().Single(l => l.ID == listingId);
            var listingDetails = PropertyListingDetails.MakeDetails(listing);
            var listingSummary = PropertyListingSummary.Summarize(listingDetails);

            var savedSearches = FilterQueryForMatchingPropertyListings(DbManager.Db.SavedPropertySearchesDbSet, listingDetails)
                .Where(sps => sps.SendPaidSmsMessages && sps.SmsNotificationTargetID.HasValue && sps.SmsNotificationTarget.IsVerified && !sps.SmsNotificationTarget.IsDeleted)
                .Include(sps => sps.User)
                .Include(sps => sps.SmsNotificationTarget)
                .ToList();

		    var matchingSearches = FindMatchingPropertySearchesForSms(savedSearches, listing, listingDetails)
		        .Select(s => new
		                     {
		                         SavedSearch = s,
		                         NotificationText = BuildPropertyListingText(listingDetails, listingSummary, s.SmsNotificationParts),
		                         Notification = new SavedPropertySearchSmsNotification
		                                        {
		                                            PropertyListingID = listingId,
		                                            SavedSearchID = s.ID,
		                                            ContactMethodID = s.SmsNotificationTargetID.Value,
		                                            CreationTime = DateTime.Now,
		                                            TargetUserID = s.UserID
		                                        }
		                     }).ToList();

            if (!matchingSearches.Any())
                return 0;

            foreach (var search in matchingSearches)
		    {
		        var userBalance = BalanceCache[search.SavedSearch.UserID];
		        UserBalanceService.OnUserCostProcessed(userBalance, search.SavedSearch.ID, NotificationSourceEntityType.SavedPropertySearchSmsNotification);

                if (!userBalance.CanSpend)
		        {
                    Log.DebugFormat("SMS for Listing {0} - Saved search {1} matched, but contact method {2} did not receive a notification due to low credit balance for the user.", listingId,
                        search.SavedSearch.ID, search.SavedSearch.SmsNotificationTargetID);
                    RealEstateStaticLogs.SavedSearch.WarnFormat("SMS: Listing {0}, Saved search {1} for user {2} contact method {3}, matched but NOT notified due to low credit balance. Number of segments: {4}",
                        listingId, search.SavedSearch.ID, search.SavedSearch.UserID, search.SavedSearch.SmsNotificationTargetID, search.Notification.NumberOfSmsSegments);
                    
                    continue;
		        }

                search.SavedSearch.NumberOfPaidSmsMessagesSent++;
                search.Notification.NumberOfSmsSegments = SmsMessageUtils.CalculateNumberOfSegments(search.NotificationText);

                DbManager.Db.SavedPropertySearchSmsNotificationsDbSet.Add(search.Notification);
                DbManager.SaveDefaultDbChanges();

                SmsMessageService.EnqueueOutgoingMessage(
                    search.NotificationText, search.SavedSearch.SmsNotificationTarget.ContactMethodText, NotificationReason.NewSavedSearchResult,
                    NotificationSourceEntityType.SavedPropertySearchSmsNotification, search.Notification.ID, targetUserId: search.SavedSearch.UserID);

                Log.DebugFormat("SMS for Listing {0} - Saved search {1} matched, and contact method {2} received a notification.", listingId, search.SavedSearch.ID, search.SavedSearch.SmsNotificationTargetID);
                RealEstateStaticLogs.SavedSearch.InfoFormat("SMS: Listing {0}, Saved search {1} for user {2} contact method {3}, matched and notified. Number of segments: {4}",
                    listingId, search.SavedSearch.ID, search.SavedSearch.UserID, search.SavedSearch.SmsNotificationTargetID, search.Notification.NumberOfSmsSegments);
            }

            Log.DebugFormat("SMS for Listing {0} - Send sms service finished, {1} notifications sent.", listingId, matchingSearches.Count);
            return matchingSearches.Count;
        }

	    public bool BillSavedSearchSmsNotifications()
	    {
	        var notifications = DbManager.Db.SavedPropertySearchSmsNotificationsDbSet
	            .Where(n => !n.BillingID.HasValue)
                .OrderBy(n => n.TargetUserID) // Order by User ID to make it likely to retrieve notifications if the same user in a single batch
                .ThenBy(n => n.CreationTime)
                .Take(BillSavedSearchSmsNotificationsBatchSize)
	            .ToList();

            var billingsToRecalc = new List<SavedSearchSmsNotificationBilling>();
	        foreach (var notification in notifications)
	        {
	            var notificationDate = notification.CreationTime.Date;

	            var billing = billingsToRecalc.FirstOrDefault(
	                b => b.TargetUserID == notification.TargetUserID &&
	                     b.NotificationsDate == notificationDate);

	            if (billing == null)
	            {
	                billing = DbManager.Db.SavedSearchSmsNotificationBillingsDbSet
	                    .FirstOrDefault(b => b.TargetUserID == notification.TargetUserID &&
	                                         b.NotificationsDate == notificationDate &&
	                                         (b.BillingState == BillingSourceEntityState.PartiallyApplied || b.BillingState == BillingSourceEntityState.Pending));
	            }

	            if (billing == null)
	            {
	                billing = new SavedSearchSmsNotificationBilling
	                          {
	                              CreationTime = DateTime.Now,
	                              NotificationsDate = notificationDate,
	                              NumberOfNotifications = 0,
	                              NumberOfNotificationParts = 0,
	                              TargetUserID = notification.TargetUserID,
	                              BillingState = BillingSourceEntityState.Pending,
	                          };

	                DbManager.Db.SavedSearchSmsNotificationBillingsDbSet.Add(billing);
	            }

	            notification.Billing = billing;
	            billing.NumberOfNotifications++;
	            billing.NumberOfNotificationParts += notification.NumberOfSmsSegments;

                if (!billingsToRecalc.Contains(billing))
                    billingsToRecalc.Add(billing);
	        }

            DbManager.SaveDefaultDbChanges();
	        UserBillingComponent.PartiallyApplyOrRecalculate<SavedSearchSmsNotificationBilling>(billingsToRecalc);

	        return notifications.Count >= BillSavedSearchSmsNotificationsBatchSize;
	    }

	    public bool FinalizePartialSavedSearchSmsNotificationBillings()
	    {
	        // Finalize items up to two days ago, so that we have enough time for delivery checking
            var maxNotificationDate = DateTime.Now.Date.AddDays(-2);

	        var billings = DbManager.Db.SavedSearchSmsNotificationBillingsDbSet
	            .Where(b => b.BillingState == BillingSourceEntityState.PartiallyApplied &&
	                        b.NotificationsDate <= maxNotificationDate)
	            .OrderBy(b => b.NotificationsDate)
	            .ThenBy(b => b.TargetUserID)
	            .Take(FinalizePartialSavedSearchSmsNotificationBillingsBatchSize)
	            .ToList();

	        foreach (var billing in billings)
	        {
	            // Load all related notifications

	            var billingId = billing.ID;
	            var targetUserId = billing.TargetUserID;
                var notificationTimeMin = billing.NotificationsDate;
	            var notificationTimeMax = billing.NotificationsDate.AddDays(1);

	            var notifications = DbManager.Db.SavedPropertySearchSmsNotificationsDbSet
	                .Where(n => (n.BillingID == billingId) || (n.CreationTime >= notificationTimeMin && n.CreationTime < notificationTimeMax && n.TargetUserID == targetUserId))
	                .ToList();

                // Iterate over the notifications and make corrections

	            foreach (var notification in notifications)
	            {
	                if (notification.CreationTime.Date != billing.NotificationsDate || notification.TargetUserID != billing.TargetUserID)
	                {
                        // If the notification is assigned to this billing by mistake, remove it.
                        // The billing scheduler will pick it up and process it later.
                        notification.BillingID = null;
	                }
                    else
                    {
                        // If the notification is assigned to another billing by mistake, 
                        // re-assign it to this billing.
                        notification.BillingID = billingId;
                    }
	            }

                // Recalculate the totals
	            billing.NumberOfNotifications = notifications.Count(n => n.BillingID == billingId);
	            billing.NumberOfNotificationParts = notifications.Where(n => n.BillingID == billingId).Sum(n => n.NumberOfSmsSegments);
	            billing.CompletionTime = DateTime.Now;
	        }

            UserBillingComponent.ApplyOrFinalizePartiallyApplied<SavedSearchSmsNotificationBilling>(billings);
	        return billings.Count >= FinalizePartialSavedSearchSmsNotificationBillingsBatchSize;
	    }

	    #endregion

		#region Private helper methods

		private static ValidationResult ValidateSavedPropertySearch(SavedPropertySearch savedPropertySearch)
		{
			if (savedPropertySearch.SendPaidSmsMessages &&
				(!savedPropertySearch.SmsNotificationParts.HasValue ||
				 savedPropertySearch.SmsNotificationParts.Value == SavedPropertySearchSmsNotificationPart.None ||
				 savedPropertySearch.SmsNotificationParts.Value == SavedPropertySearchSmsNotificationPart.SiteName))
			{
				return ValidationResult.Failure("SmsNotificationParts", GeneralValidationErrors.ValueNotSpecified);
			}

			if (savedPropertySearch.SendNotificationEmails && !savedPropertySearch.EmailNotificationType.HasValue)
			{
				return ValidationResult.Failure("EmailNotificationType", GeneralValidationErrors.ValueNotSpecified);
			}

			// Validate if EmailNotificationTargetID and SmsNotificationTargetID belongs to the user
			return ValidationResult.Success;
		}

		private static void OnUpdate(SavedPropertySearch savedPropertySearch)
		{
			if (!(savedPropertySearch.SendNotificationEmails || savedPropertySearch.SendPaidSmsMessages || savedPropertySearch.SendPromotionalSmsMessages))
				savedPropertySearch.SendNotificationsUntil = null;
		}

		private IQueryable<SavedPropertySearch> FilterQueryForMatchingPropertyListings(IQueryable<SavedPropertySearch> query, PropertyListingDetails listing)
		{
            var parentVicinityIds = new List<long>();
		    if (listing.VicinityID.HasValue)
		    {
		        var listingVicinity = VicinityCache[listing.VicinityID.Value];
                if (listingVicinity != null)
                    parentVicinityIds.AddRange(listingVicinity.GetParentIDsInclusive());
		    }

			return query.Where(sps =>
				sps.User.IsEnabled &&
				!sps.DeleteTime.HasValue &&
				(!sps.PropertyType.HasValue || sps.PropertyType.Value == listing.PropertyType) &&
				(!sps.IntentionOfOwner.HasValue || sps.IntentionOfOwner.Value == listing.IntentionOfOwner) &&
				(!sps.GeographicRegions.Any() || sps.GeographicRegions.Any(gr => parentVicinityIds.Contains(gr.VicinityID))));
		}

	    private SavedPropertySearchUpdateHistory ExtractChanges(SavedPropertySearch search)
	    {
	        var changes = EntityChangeExtractor.ExtractChanges(DbManager.Db, new[]
	        {
	            new EntityChangeSource {Entity = search}
	        });

	        var changeCollection = new EntityChangeCollection {Changes = changes};
	        var serializer = new XmlSerializer(typeof (EntityChangeCollection));
	        var writer = new StringWriter();

	        serializer.Serialize(writer, changeCollection);
	        string serializedChanges = writer.ToString();

	        return new SavedPropertySearchUpdateHistory
	        {
	            SavedSearchID = search.ID,
	            UpdateTime = DateTime.Now,
	            UpdateDetails = serializedChanges,
	            SessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID)),
	            UserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault()
	        };
	    }

	    private IEnumerable<SavedPropertySearch> FindMatchingPropertySearchesForEmail(
	        List<SavedPropertySearch> savedSearches,
	        PropertyListing listing,
	        PropertyListingDetails listingDetails)
	    {
            var lastWeek = DateTime.Now.AddDays(-7);
            var recentNotifications = new HashSet<long>(
                DbManager.Db.SavedPropertySearchEmailNotifications
                    .Where(n => n.PropertyListingID == listing.ID && n.CreationDate > lastWeek)
                    .Select(n => n.ContactMethodID));

	        return FindMatchingPropertySearches(savedSearches, listing, listingDetails, recentNotifications, ss => ss.EmailNotificationTargetID, "Email");
	    }

	    private IEnumerable<SavedPropertySearch> FindMatchingPropertySearchesForSms(
	        List<SavedPropertySearch> savedSearches,
	        PropertyListing listing,
	        PropertyListingDetails listingDetails)
	    {
            var lastWeek = DateTime.Now.AddDays(-7);
            var recentNotifications = new HashSet<long>(
                DbManager.Db.SavedPropertySearchSmsNotifications
                    .Where(n => n.PropertyListingID == listing.ID && n.CreationTime > lastWeek)
                    .Select(n => n.ContactMethodID));

	        return FindMatchingPropertySearches(savedSearches, listing, listingDetails, recentNotifications, ss => ss.SmsNotificationTargetID, "SMS");
	    }

	    private IEnumerable<SavedPropertySearch> FindMatchingPropertySearches(
            List<SavedPropertySearch> savedSearches, 
            PropertyListing listing, 
            PropertyListingDetails listingDetails, 
            HashSet<long> recentNotifications,
            Func<SavedPropertySearch, long?> targetContactMethodIdFunc,
            string reason)
	    {
            Log.DebugFormat("{0} for Listing {1} - a total of {2} saved searches found as initial match.", reason, listing.ID, savedSearches.Count);
            Log.DebugExt(() =>
                reason + " for Listing " + listing.ID + " - Recent contact method notified for this listing are: " +
                string.Join(",", recentNotifications.Select(id => id.ToString(CultureInfo.InvariantCulture))));

            var result = new List<SavedPropertySearch>();

            foreach (var savedSearch in savedSearches)
            {
                var search = PropertySearchUtil.BuildPropertySearch(savedSearch);
                if (!PropertySearchUtil.MatchListing(search, listing, false))
                    continue;

                // Ignore the listing if the saved search owner has created it
                if (listingDetails.OwnerUserID.HasValue && listingDetails.OwnerUserID.Value == savedSearch.UserID)
                    continue;

                var targetContactMethodId = targetContactMethodIdFunc(savedSearch).GetValueOrDefault();

                // Ignore notification if the listing has already been sent to the same
                // user contact method recently
                if (recentNotifications.Contains(targetContactMethodId))
                {
                    Log.DebugFormat(
                        "{0} for Listing {1} - Saved search {2} matched, but didn't get notifications because contact method ID {3} has recently received the same notification.",
                        reason, listing.ID, savedSearch.ID, targetContactMethodId);

                    RealEstateStaticLogs.SavedSearch.InfoFormat("{0}: Listing {1}, Saved search {2} for user {3} contact method {4}, matched but SKIPPED.",
                        reason, listing.ID, savedSearch.ID, savedSearch.UserID, targetContactMethodId);

                    continue;
                }

                recentNotifications.Add(targetContactMethodId);
                result.Add(savedSearch);
            }

            return result;
	    }

        private string BuildPropertyListingText(PropertyListingDetails listing, PropertyListingSummary listingSummary, SavedPropertySearchSmsNotificationPart? smsNotificationParts)
        {
            if (!smsNotificationParts.HasValue)
                throw new ArgumentNullException("smsNotificationParts");
            if (listing == null)
                throw new ArgumentNullException("listing");
            if (listingSummary == null)
                listingSummary = PropertyListingSummary.Summarize(listing);

            var messageParts = new List<string>();

            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ListingCode))
                messageParts.Add(string.Format(SavedSearchServiceResources.PropertyListing_ListingCode, listing.Code));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PropertyType))
                messageParts.Add(PropertyPresentationHelper.BuildShortTitle(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.NumberOfRooms))
                messageParts.Add(PropertyPresentationHelper.BuildNumberOfRooms(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ShortGeographicRegion) && !smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.LongGeographicRegion))
                messageParts.Add(PropertyPresentationHelper.BuildShortRegion(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.LongGeographicRegion))
                messageParts.Add(PropertyPresentationHelper.BuildRegionString(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.UserEnteredAddress))
                messageParts.Add(PropertyPresentationHelper.BuildUserEnteredAddress(listing).Truncate(50));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.Area))
                messageParts.Add(PropertyPresentationHelper.BuildAreaString(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PriceOrRent))
                messageParts.Add(PropertyPresentationHelper.BuildPriceString(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PerAreaPrice))
                messageParts.Add(PropertyPresentationHelper.BuildPricePerAreaString(listingSummary));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactName))
                messageParts.Add(listing.ContactInfo.ContactName.Truncate(25));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactPhone))
                messageParts.Add(listing.ContactInfo.ContactPhone1.RemoveWhitespace().Truncate(18));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactSecondPhone))
                messageParts.Add(listing.ContactInfo.ContactPhone2.RemoveWhitespace().Truncate(18));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ListingUrl))
                messageParts.Add(string.Format(SavedSearchServiceResources.PropertyListing_DetailsUrl, listing.Code));

            var messageText = string.Join(GeneralResources.Comma, messageParts.Where(s => !string.IsNullOrWhiteSpace(s)));
            if (smsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.SiteName))
                messageText = SavedSearchServiceResources.PropertyListing_SiteNameString + messageText;

            return messageText;
        }

	    #endregion
	}
}