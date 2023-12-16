using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.EF;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Notification.Sms;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Core.Services.Enums;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.DomainUtil;
using log4net;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class PropertyService : IPropertyService
    {
        private const int MaxNumberOfSponsoredListings = 3;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        [ComponentPlug]
        public ISponsorshipCalculatorComponent SponsorshipComponent { get; set; }

        [ComponentPlug]
        public ITarrifService TarrifService { get; set; }

        [ComponentPlug]
        public static IVicinityCache VicinityCache { get; set; }


        private static readonly ILog Log = LogManager.GetLogger(typeof (FarapayamakSenderLinesCache));

        #region Implementation of IPropertyService

        public ValidationResult Save(PropertyListing listing)
        {
            if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var validationResult = ValidateForSave(listing);
            if (!validationResult.IsValid)
                return validationResult;

            var validationContactInfoResult = ValidateContactInfo(listing, DbManager.Db);

            if (!validationContactInfoResult.IsValid)
                return validationContactInfoResult;

            OnUpdate(listing, null);
            UpdateEditPasswordInternal(listing);

            listing.CreationDate = DateTime.Now;
            listing.CreatorSessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID));
            listing.CreatorUserID = ServiceContext.Principal.CoreIdentity.IsAuthenticated
                ? ServiceContext.Principal.CoreIdentity.UserId
                : null;
            listing.OwnerUserID = (ServiceContext.Principal.CoreIdentity.IsAuthenticated &&
                                   !ServiceContext.Principal.IsOperator)
                ? ServiceContext.Principal.CoreIdentity.UserId
                : null;

            DbManager.Db.PropertyListingsDbSet.Add(listing);
            DbManager.SaveDefaultDbChanges();

            EmailNotificationService.SendPropertyRegistrationInfoToOwner(listing.ContactInfo, LoadSummary(listing.ID),
                listing.EditPassword);

            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listing.ID, ActivityAction.Create);

            return ValidationResult.Success;
        }

        public ValidationResult Update(long listingID, Func<PropertyListing, EntityUpdateResult> updateFunction)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var db = DbManager.GetFreshDb();

            var listing = db.PropertyListingsDbSet.IncludeInfoProperties().SingleOrDefault(l => l.ID == listingID);
            if (listing == null)
                throw new ArgumentException("Listing does not exist");

            if (listing.DeleteDate.HasValue)
                throw new ArgumentException("Listing is already deleted.");

            var entityUpdateResult = updateFunction(listing);

            var validationContactInfoResult = ValidateContactInfo(listing, db);
            if (!validationContactInfoResult.IsValid)
                return validationContactInfoResult;

            if (entityUpdateResult.CancelUpdate)
            {
                DbManager.CancelDbChanges(db);
                return ValidationResult.Success;
            }

            var validationResult = listing.IsPublished()
                ? ValidateForPublish(PropertyListingDetails.MakeDetails(listing))
                : ValidateForSave(listing);
            if (!validationResult.IsValid)
            {
                DbManager.CancelDbChanges(db);
                return validationResult;
            }

            db.ChangeTracker.DetectChanges();
            var changes = ExtractChanges(db, listing);
            var updateHistory = BuildUpdateHistory(changes, listing);
            db.PropertyListingUpdateHistoriesDbSet.Add(updateHistory);

            OnUpdate(listing, changes);

            if (listing.SaleConditions != null)
                listing.SaleConditions.RecalculatePrices(listing.Estate == null ? null : listing.Estate.Area,
                    listing.Unit == null ? null : listing.Unit.Area);

            DbManager.SaveDbChanges(db);
            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingID, ActivityAction.Edit,
                auditEntity: AuditEntityType.PropertyListingUpdateHistory, auditEntityID: updateHistory.ID);

            return ValidationResult.Success;
        }

        public bool ValidateEditPassword(long propertyListingId, string editPassword)
        {
            var listing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == propertyListingId);
            if (listing == null)
                return false;

            var result = listing.EditPassword.Trim() == editPassword.Trim();

            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, propertyListingId,
                ActivityAction.Authenticate, ActivityLogDetails.PropertyListingActionDetails.ValidateEditPassword,
                result);
            return result;
        }

        public void SetOwner(IEnumerable<long> propertyIds, long userId)
        {
            var propertyListings = DbManager.Db.PropertyListingsDbSet.Where(l => propertyIds.Contains(l.ID)).ToList();
            foreach (var propertyListing in propertyListings)
            {
                if (propertyListing.DeleteDate.HasValue)
                    continue;

                propertyListing.OwnerUserID = userId;
                propertyListing.ModificationDate = DateTime.Now;

                ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, propertyListing.ID,
                    ActivityAction.Other, ActivityLogDetails.PropertyListingActionDetails.AcquireOwnership);
            }
        }

        public void SetApproved(long listingId, bool? approved)
        {
            var listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(l => l.ID == listingId);
            if (listing == null)
                throw new ArgumentException("PropertyListing not found.");

            if (listing.DeleteDate.HasValue)
                return;

            if (listing.Approved == approved)
                return;

            listing.Approved = approved;
            listing.ModificationDate = DateTime.Now;

            if (!approved.HasValue)
            {
                ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingId, ActivityAction.Edit,
                    ActivityLogDetails.PropertyListingActionDetails.ClearApproval);
            }
            else
            {
                ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingId,
                    approved.Value ? ActivityAction.Approve : ActivityAction.Reject);
                ActivityLogService.MarkTaskComplete(TargetEntityType.PropertyListing, listingId, ActivityAction.Create);
                ActivityLogService.MarkTaskComplete(TargetEntityType.PropertyListing, listingId, ActivityAction.Edit);
            }
        }

        public ValidationResult Publish(long listingID, int days)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var listing =
                DbManager.Db.PropertyListingsDbSet.IncludeInfoProperties().SingleOrDefault(l => l.ID == listingID);
            if (listing == null)
                throw new ArgumentException("Invalid listing ID");

            if (listing.DeleteDate.HasValue)
                throw new ArgumentException("Listing is already deleted.");

            var validationResult = ValidateForPublish(PropertyListingDetails.MakeDetails(listing));
            if (!validationResult.IsValid)
                return validationResult;

            if (days > 150 || days < 1)
                return ValidationResult.Failure("INVALID_NUMBER_OF_DAYS");

            // Update the publish date only if this is a new publish.
            // If it is a re-publish of a still-valid publish, don't change the publish date.

            if (!listing.PublishEndDate.HasValue || listing.PublishEndDate < DateTime.Now)
                listing.PublishDate = DateTime.Now;

            listing.ModificationDate = DateTime.Now;
            listing.PublishEndDate = DateTime.Now.Date.AddDays(days + 1);

            var history = new PropertyListingPublishHistory
            {
                PropertyListingID = listing.ID,
                PreviousPublishDate = listing.PublishDate,
                PreviousPublishEndDate = listing.PublishEndDate,
                PublishDate = DateTime.Now,
                PublishDays = days,
                SessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID)),
                UserID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => r.UserID))
            };

            DbManager.Db.PropertyListingPublishHistoriesDbSet.Add(history);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingID, ActivityAction.Publish,
                auditEntity: AuditEntityType.PropertyListingPublishHistory, auditEntityID: history.ID);
            return ValidationResult.Success;
        }

        public void UnPublish(long listingID)
        {
            // TODO: Check permission

            var listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(l => l.ID == listingID);
            if (listing == null)
                throw new ArgumentException("Invalid listing ID");

            if (listing.DeleteDate.HasValue)
                throw new ArgumentException("Listing is already deleted.");

            listing.PublishEndDate = DateTime.Now;
            listing.ModificationDate = DateTime.Now;

            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingID, ActivityAction.Unpublish);
        }

        public ToggleFavoriteResult ToggleFavorite(long listingID)
        {
            if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            if (ServiceContext.Principal == null || ServiceContext.Principal.CoreIdentity == null ||
                !ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                throw new InvalidOperationException("Can't toggle favorite for an auauthenticated session.");

            var userId = ServiceContext.Principal.CoreIdentity.UserId.Value;
            var currentRecord =
                DbManager.Db.UserFavoritePropertyListingsDbSet.SingleOrDefault(
                    f => f.ListingID == listingID && f.UserID == userId);
            var result = new ToggleFavoriteResult();

            if (currentRecord != null)
            {
                DbManager.Db.UserFavoritePropertyListingsDbSet.Remove(currentRecord);
                result.IsMarkedAsFavorite = false;
            }
            else
            {
                currentRecord = new UserFavoritePropertyListing
                {
                    ListingID = listingID,
                    UserID = userId,
                    CreationDate = DateTime.Now,
                    CreationSessionID = ServiceContext.CurrentSession.Record.ID
                };

                DbManager.Db.UserFavoritePropertyListingsDbSet.Add(currentRecord);
                result.IsMarkedAsFavorite = true;
            }

            DbManager.SaveDefaultDbChanges();
            result.TimesMarkedAsFavorite = DbManager.Db.UserFavoritePropertyListings.Count(f => f.ListingID == listingID);
            return result;
        }

        public ValidationResult ValidateForSave(PropertyListing listing)
        {
            return ValidationResult.Success;
        }

        public ValidationResult ValidateForPublish(PropertyListingDetails listing)
        {
            var result = new List<ValidationError>();

            ValidateEstateForPublish(listing, result);
            ValidateBuildingForPublish(listing, result);
            ValidateUnitForPublish(listing, result);
            ValidateSaleConditionsForPublish(listing, result);
            ValidateRentConditionsForPublish(listing, result);
            ValidateContactInfoForPublish(listing, result);
            ValidateListingPropertiesForPublish(listing, result);

            return new ValidationResult {Errors = result.Count > 0 ? result : null};
        }

        public void Delete(long listingID)
        {
            var listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(l => l.ID == listingID);
            if (listing == null || listing.DeleteDate.HasValue)
                return;

            listing.DeleteDate = DateTime.Now;
            ActivityLogService.ReportActivity(TargetEntityType.PropertyListing, listingID, ActivityAction.Delete);
        }

        public PropertyListingDetails Load(long listingID, bool includeEstate = false, bool includeBuilding = false,
            bool includeUnit = false, bool includeSaleAndRentConditions = false,
            bool includeContactInfo = false, bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return PropertyListingDetails.MakeDetails(
                    DbManager.Db.PropertyListingsDbSet.Where(l => l.ID == listingID),
                    includeEstate,
                    includeBuilding,
                    includeUnit,
                    includeSaleAndRentConditions,
                    includeContactInfo)
                    .SingleOrDefault();
            }
            return PropertyListingDetails.MakeDetails(
                DbManager.Db.PropertyListings.Where(l => l.ID == listingID),
                includeEstate,
                includeBuilding,
                includeUnit,
                includeSaleAndRentConditions,
                includeContactInfo)
                .SingleOrDefault();
        }

        public PropertyListingDetails LoadWithAllDetails(long listingID, bool includeDeleted = false)
        {
            return Load(listingID, true, true, true, true, true, includeDeleted);
        }

        public PropertyListingDetails LoadForVisit(long listingID)
        {
            var result = LoadWithAllDetails(listingID);

            if (result == null)
                return null;

            if (!ServiceContext.CurrentSession.IsCrawler && !ServiceContext.Principal.IsOperator)
            {
                if (ServiceContext.CurrentSession != null &&
					ServiceContext.CurrentSession.Record != null &&
					result.CreatorSessionID != ServiceContext.CurrentSession.Record.ID &&
                    (result.OwnerUserID == null || result.OwnerUserID != ServiceContext.CurrentSession.Record.UserID))
                {
                    DbManager.Db.Database.ExecuteSqlCommand(
                        "UPDATE [PropertyListing] SET [NumberOfVisits] = [NumberOfVisits] + 1 WHERE ID = " + listingID);
                }
            }

            return result;
        }

        public PropertyListingDetails LoadForContactInfo(long listingID)
        {
            // Use DbSet to allow operator to see the info on deleted records
            var result =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListingsDbSet.Where(l => l.ID == listingID),
                    includeContactInfo: true).SingleOrDefault();

            if (result == null)
                return null;

            if (!ServiceContext.CurrentSession.IsCrawler && !ServiceContext.Principal.IsOperator)
            {
                if (!ServiceContext.CurrentSession.IsValidSession ||
                    // For service-call requests. Probably need to change this to checking the request channel
                    (result.CreatorSessionID != ServiceContext.CurrentSession.Record.ID &&
                     (result.OwnerUserID == null || result.OwnerUserID != ServiceContext.CurrentSession.Record.UserID)))
                {
                    DbManager.Db.Database.ExecuteSqlCommand(
                        "UPDATE [PropertyListing] SET [NumberOfContactInfoRetrievals] = [NumberOfContactInfoRetrievals] + 1 WHERE ID = " +
                        listingID);
                }
            }

            return result;
        }

        public PropertyListingSummary LoadSummary(long listingID)
        {
            return
                PropertyListingSummary.Summarize(DbManager.Db.PropertyListings.Where(l => l.ID == listingID))
                    .SingleOrDefault();
        }

        public List<PropertyListingSummary> LoadSummaries(IEnumerable<long> listingIDs)
        {
            return
                PropertyListingSummary.Summarize(DbManager.Db.PropertyListings.Where(l => listingIDs.Contains(l.ID)))
                    .ToList();
        }

        public SearchResult RunSearch(PropertySearch search, int startIndex, int pageSize, bool includeSponsored,
            bool incrementNumberOfSearches)
        {
            if (startIndex < 0)
                return null;

            if (pageSize > 1000)
                throw new ArgumentException("Can't query for more than 1000 results at a time.");

            IQueryable<PropertyListing> query =
                DbManager.Db.PropertyListings.Where(PropertyListingExtensions.GetPublicListingExpression());

            var result = new SearchResult
            {
                PageSize = pageSize,
                SponsoredResults = new List<SponsoredPropertyListingSummary>()
            };

            query = PropertySearchUtil.ApplySearchQuery(search, query);

            if (includeSponsored)
            {
                var isFirstPage = startIndex < 1;
                ExtractSponsoredSearchResults(query, isFirstPage, result.SponsoredResults);
            }

            query = PropertySearchUtil.ApplySearchOrder(search, query);
            result.TotalNumberOfRecords = query.Count();
            result.IndexOfFirstResult = startIndex;
            result.IndexOfLastResult = startIndex;

            if (startIndex > result.TotalNumberOfRecords)
                return result;

            result.PageResults =
                PropertyListingSummary.Summarize(query).Skip(result.IndexOfFirstResult).Take(result.PageSize).ToList();
            result.IndexOfLastResult = result.IndexOfFirstResult + result.PageResults.Count - 1;

            if (incrementNumberOfSearches && ServiceContext.CurrentSession != null &&
                ServiceContext.CurrentSession.IsValidSession &&
                !ServiceContext.CurrentSession.IsCrawler && !ServiceContext.Principal.IsOperator)
            {
                // TODO: Change SearchHistory to keep start index instead of page number
                int page = (startIndex + pageSize - 1)/pageSize;
                DbManager.Db.PropertySearchHistoriesDbSet.Add(BuildSearchHistory(search, page));
                IncrementNumberOfSearches(result.PageResults);
                IncrementNumberOfSearches(result.SponsoredResults.Select(s => s.PropertyListingSummary));
            }

            return result;
        }

        public long? FindSearchIndex(PropertySearch search, int index)
        {
            if (index <= 0)
                return null;

            IQueryable<PropertyListing> query =
                DbManager.Db.PropertyListings.Where(PropertyListingExtensions.GetPublicListingExpression());
            query = PropertySearchUtil.ApplySearchQuery(search, query);
            query = PropertySearchUtil.ApplySearchOrder(search, query);

            var idQuery = query.Select(l => l.ID);
            if (index > 0)
                idQuery = idQuery.Skip(index - 1);

            return idQuery.FirstOrDefault();
        }

        public PagedList<PropertyListingSummary> ListingsOfUser(long? userId, bool publicOnly, bool publishedOnly,
            int page, int pageSize)
        {
            IQueryable<PropertyListing> query = DbManager.Db.PropertyListings;

            if (userId.HasValue)
                query = query.Where(l => l.OwnerUserID == userId.Value);

            if (publicOnly)
                query = query.Where(PropertyListingExtensions.GetPublicListingExpression());
            else if (publishedOnly)
                query = query.Where(PropertyListingExtensions.GetPublishedListingExpression());

            int count = query.Count();
            var result = PagedList<PropertyListingSummary>.BuildUsingPageNumber(count, pageSize, page);

            query = query.OrderByDescending(l => l.ID);
            result.FillFrom(PropertyListingSummary.Summarize(query));

            return result;
        }

        #endregion

        #region Private helper methods

        private void OnUpdate(PropertyListing listing, EntityChangeCollection changeCollection)
        {
            listing.ModificationDate = DateTime.Now;

            UpdateApproved(listing, changeCollection);
            DbGeography centerPoint = null;

            if (listing.Estate != null)
            {
				if (listing.Estate.VicinityID != null)
				{
					Vicinity vicinity = VicinityCache[listing.Estate.VicinityID.Value];

					if (vicinity != null)
					{
						var hierarchyString = string.Join("/", vicinity.GetParentIDsInclusive());
						listing.VicinityHierarchyString = "/" + hierarchyString + "/";
						centerPoint = vicinity.CenterPoint ??
									  vicinity.Boundary.FindBoundingBox().IfNotNull(bb => bb.GetCenter().ToDbGeography());
					}
				}

				if (listing.Estate.VicinityID != null && centerPoint != null)
                {
                    if (!(listing.Estate.GeographicLocationType.HasValue) ||
                        listing.Estate.GeographicLocationType ==
                        GeographicLocationSpecificationType.InferredFromVicinity ||
                        listing.Estate.GeographicLocationType ==
                        GeographicLocationSpecificationType.InferredFromVicinityAndAddress)
                    {
                        listing.Estate.GeographicLocation = centerPoint;
                        listing.Estate.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
                    }
                }

                listing.VicinityID = listing.Estate.VicinityID;
                listing.GeographicLocation = listing.Estate.GeographicLocation;
                listing.GeographicLocationType = listing.Estate.GeographicLocationType;
            }

            if (listing.SaleConditions != null)
            {
                listing.SaleConditions.RecalculatePrices(listing.Estate == null ? null : listing.Estate.Area,
                    listing.Unit == null ? null : listing.Unit.Area);
                listing.NormalizedPrice = listing.SaleConditions.Price.GetValueOrDefault();
            }
            else if (listing.RentConditions != null)
            {
                // Convert rent to an estimated property price:
                // Each 25K rent => 1M Mortgage
                // Price estimate = Mortgage * 5

                listing.NormalizedPrice = (listing.RentConditions.Rent.GetValueOrDefault()*40 +
                                           listing.RentConditions.Mortgage.GetValueOrDefault())*5;
            }
            else
            {
                listing.NormalizedPrice = null;
            }

            if (listing.NormalizedPrice.HasValue)
            {
                listing.NormalizedPricePerEstateArea = (listing.Estate != null && listing.Estate.Area.HasValue)
                    ? listing.NormalizedPrice/listing.Estate.Area.Value
                    : null;
                listing.NormalizedPricePerUnitArea = (listing.Unit != null && listing.Unit.Area.HasValue)
                    ? listing.NormalizedPrice/listing.Unit.Area.Value
                    : null;
            }
        }

        private static void UpdateApproved(PropertyListing listing, EntityChangeCollection changeCollection)
        {
            if (listing == null)
                return;

            // Ignoring approval procedure: do not change the approved flag if already approved
            if (listing.Approved.HasValue && listing.Approved.Value)
                return;

            // Ignoring approval procedure: mark it as approved if the listing is new
            if (changeCollection == null)
            {
                listing.Approved = true;
                return;
            }

            //TODO: When contactInfoId changes & other boxes change by the javascript method, the extract changes method only find out about changes of contactInfoId not the boxes

            // Ignore the change if none of the changed values are a string, and the listing is previously approved
            if (!changeCollection.Changes.Any(e => e.ChangedValue is string) && listing.Approved.HasValue &&
                listing.Approved.Value)
                return;

            // If the user is an operator, no need to re-approve
            if (ServiceContext.Principal.IsOperator && listing.Approved.HasValue && listing.Approved.Value)
                return;

            listing.Approved = null;
        }

        private static EntityChangeCollection ExtractChanges(DbContext db, PropertyListing listing)
        {
            var changes = EntityChangeExtractor.ExtractChanges(db, new[]
            {
                new EntityChangeSource {Entity = listing},
                new EntityChangeSource {Entity = listing.Estate, Prefix = "Estate."},
                new EntityChangeSource {Entity = listing.Building, Prefix = "Building."},
                new EntityChangeSource {Entity = listing.Unit, Prefix = "Unit."},
                new EntityChangeSource {Entity = listing.SaleConditions, Prefix = "SaleConditions."},
                new EntityChangeSource {Entity = listing.RentConditions, Prefix = "RentConditions."},
                new EntityChangeSource {Entity = listing.ContactInfo, Prefix = "ContactInfo."}
            });

            return new EntityChangeCollection {Changes = changes};
        }

        private static PropertyListingUpdateHistory BuildUpdateHistory(EntityChangeCollection changeCollection,
            PropertyListing listing)
        {
            return new PropertyListingUpdateHistory
            {
                PropertyListingID = listing.ID,
                UpdateTime = DateTime.Now,
                UpdateDetails = changeCollection.ToJsv(),
                SessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID)),
                UserID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => r.UserID))
            };
        }

        private static void UpdateEditPasswordInternal(PropertyListing listing)
        {
            // Generate a 6-digit random number 
            var random = new Random((int) (DateTime.Now.Ticks & 0x0FFFFFFF));
            var newPassword = random.Next(100000, 999999);
            listing.EditPassword = newPassword.ToString(CultureInfo.InvariantCulture);
        }

        private static PropertySearchHistory BuildSearchHistory(PropertySearch search, int page)
        {
            return new PropertySearchHistory
            {
                SearchDate = DateTime.Now,
                SearchQuery = search.Query,
                RequestedPage = page,
                SessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID)),
                UserID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => r.UserID)),
                SortOrder = search.SortOrder,
                PropertyType = search.PropertyType,
                IntentionOfOwner = search.IntentionOfOwner,
            };
        }

        private void IncrementNumberOfSearches(IEnumerable<PropertyListingSummary> results)
        {
            if (results.Any())
            {
                var resultIds = results.Select(l => l.ID);
                DbManager.Db.Database.ExecuteSqlCommand(
                    "UPDATE [PropertyListing] SET [NumberOfSearches] = [NumberOfSearches] + 1 WHERE ID IN (" +
                    string.Join(", ", resultIds) + ")");
            }
        }

        private void ExtractSponsoredSearchResults(IQueryable<PropertyListing> query, bool isFirstPage,
            List<SponsoredPropertyListingSummary> sponsoredResults)
        {
            if (ServiceContext.CurrentSession == null || !ServiceContext.CurrentSession.IsValidSession ||
                ServiceContext.CurrentSession.IsCrawler || ServiceContext.Principal.IsOperator)
            {
                // No sponsored results in these conditions, just return
                return;
            }

            var now = DateTime.Now;
            var searchSponsorships =
                query.Select(
                    p => p.Sponsorships.Where(s => s.SponsoredEntity.Enabled &&
                                                   !s.SponsoredEntity.BlockedForLowCredit &&
                                                   !s.SponsoredEntity.DeleteTime.HasValue &&
                                                    (!s.Listing.PublishEndDate.HasValue ||
                                                 s.Listing.PublishEndDate.Value > now) &&
//                                                   s.Listing.PublishEndDate.Value >= s.SponsoredEntity.ExpirationTime.Value &&
                                                   (!s.SponsoredEntity.ExpirationTime.HasValue ||
                                                    s.SponsoredEntity.ExpirationTime.Value > now) &&
                                                 
                                                   s.Approved.HasValue && s.Approved.Value &&
                                                   !s.IgnoreSearchQuery &&
                                                   (isFirstPage || s.ShowInAllPages))
                        .OrderByDescending(s => s.SponsoredEntity.MaxPayPerImpression)
                        .FirstOrDefault())
                    .Where(s => s != null)
                    .OrderByDescending(s => s.SponsoredEntity.SelectionProbabilityWeight)
                    .ThenBy(s => s.SponsoredEntity.LastImpressionTime)
                    .Distinct()
                    .Include(s => s.SponsoredEntity)
                     .Include(s => s.Listing)
                    .Take(MaxNumberOfSponsoredListings * 2)
                    .ToList();



            var globalSponsorships = DbManager.Db.SponsoredPropertyListings
                .Where(s => s.SponsoredEntity.Enabled &&
                            !s.SponsoredEntity.BlockedForLowCredit &&
                            !s.SponsoredEntity.DeleteTime.HasValue &&
                            (!s.Listing.PublishEndDate.HasValue ||
                             s.Listing.PublishEndDate.Value > now) &&
//                          ((s.SponsoredEntity.ExpirationTime.HasValue  && s.SponsoredEntity.ExpirationTime.Value <= s.Listing.PublishEndDate.Value ) || !s.SponsoredEntity.ExpirationTime.HasValue) &&
                            (!s.SponsoredEntity.ExpirationTime.HasValue ||
                             s.SponsoredEntity.ExpirationTime.Value > now) &&
                            
                            s.Approved.HasValue && s.Approved.Value &&
                            s.IgnoreSearchQuery &&
                            (isFirstPage || s.ShowInAllPages))
                .OrderByDescending(s => s.SponsoredEntity.SelectionProbabilityWeight)
                .ThenBy(s => s.SponsoredEntity.LastImpressionTime)
                .Include(s => s.SponsoredEntity)
                .Include(s => s.Listing)
                .Take(MaxNumberOfSponsoredListings * 2)
                .ToList();

            var sponsorships = searchSponsorships.Concat(globalSponsorships);
            if (sponsorships.Any())
            {
                var sponsoredResultImpressions = SponsorshipComponent.SelectAndCalculateImpressions(sponsorships, TarrifService.GetTarrif(null).PropertyListingSponsorshipTarrif, MaxNumberOfSponsoredListings);
                var sponsoredResultIds = sponsoredResultImpressions.Select(s => s.Item1.ListingID).ToList();
                var sponsoredResultSummaries =
                    PropertyListingSummary.Summarize(
                        DbManager.Db.PropertyListings.Where(p => sponsoredResultIds.Contains(p.ID))).ToList();

                foreach (var tuple in sponsoredResultImpressions)
                {
                    var sponsoredResult = sponsoredResultSummaries.SingleOrDefault(p => p.ID == tuple.Item1.ListingID);
                    if (sponsoredResult == null)
                    {
                        Log.Warn("SponsoredResult is empty. sponsorships Id = " + tuple.Item1.ID);
                        continue;
                    }

                    if (sponsoredResult.OwnerUserID.HasValue)
                    {
                        tuple.Item2.ContentOwnerUserID = sponsoredResult.OwnerUserID.Value;
                    }

                    sponsoredResults.Add(new SponsoredPropertyListingSummary
                    {
                        PropertyListingSummary = sponsoredResult,
                        SponsoredEntity = tuple.Item1.SponsoredEntity,
                        SponsoredPropertyListing = tuple.Item1,
                        SponsoredEntityImpression = tuple.Item2
                    });
                }

                DbManager.Db.SponsoredEntityImpressionsDbSet.AddAll(sponsoredResultImpressions.Select(t => t.Item2));
            }
        }

        #endregion

        #region Validation methods

        private static void ValidateEstateForPublish(PropertyListingDetails listing, List<ValidationError> result)
        {
            if (listing.Estate == null)
                result.Add(new ValidationError(PropertyValidationErrors.EstateIsNull));
            else
            {
                if (!listing.Estate.VicinityID.HasValue)
                {
                    result.Add(new ValidationError("Estate.Vicinity", GeneralValidationErrors.ValueNotSpecified));
                }
                else
                {
                    var vicinityFromCache = VicinityCache[listing.Estate.VicinityID.Value];
                    if (vicinityFromCache == null)
                    {
                        result.Add(new ValidationError("Estate.Vicinity", GeneralValidationErrors.ValueNotSpecified));
                    }
                    else
                    {
                        if (listing.Estate.GeographicLocationType !=
                            GeographicLocationSpecificationType.UserSpecifiedExact)
                        {
                            if (!vicinityFromCache.CanContainPropertyRecords)
                                result.Add(new ValidationError("Estate.Vicinity",
                                    PropertyValidationErrors.SelectedVicinityCannotContainPropertyRecords));
                            if (vicinityFromCache.GetParentsInclusive().Any(v => !v.Enabled))
                                result.Add(new ValidationError("Estate.Vicinity",
                                    GeneralValidationErrors.ValueTargetIsDisabled));
                        }
                    }
                }

                if (listing.PropertyType.IsEstateSignificant())
                {
                    if (!listing.Estate.Area.HasValue)
                        result.Add(new ValidationError("Estate.Area", GeneralValidationErrors.ValueNotSpecified));

                    if (!listing.Estate.Direction.HasValue)
                        result.Add(new ValidationError("Estate.Direction", GeneralValidationErrors.ValueNotSpecified));

                    if (!listing.Estate.VoucherType.HasValue)
                        result.Add(new ValidationError("Estate.VoucherType", GeneralValidationErrors.ValueNotSpecified));
                }
            }
        }

        private static void ValidateBuildingForPublish(PropertyListingDetails listing, List<ValidationError> result)
        {
            if (listing.PropertyType.HasBuilding())
            {
                if (listing.Building == null)
                    result.Add(new ValidationError(PropertyValidationErrors.BuildingIsNull));
            }
        }

        private static void ValidateUnitForPublish(PropertyListingDetails listing, List<ValidationError> result)
        {
            if (listing.PropertyType.HasUnit())
            {
                if (listing.Unit == null)
                    result.Add(new ValidationError(PropertyValidationErrors.UnitIsNull));
                else
                {
                    if (!listing.Unit.UsageType.HasValue)
                        result.Add(new ValidationError("Unit.UsageType", GeneralValidationErrors.ValueNotSpecified));

                    if (!listing.Unit.Area.HasValue)
                        result.Add(new ValidationError("Unit.Area", GeneralValidationErrors.ValueNotSpecified));

                    if (!listing.Unit.NumberOfRooms.HasValue && listing.PropertyType != PropertyType.Shop)
                        result.Add(new ValidationError("Unit.NumberOfRooms", GeneralValidationErrors.ValueNotSpecified));
                }
            }
        }

        private static void ValidateSaleConditionsForPublish(PropertyListingDetails listing,
            List<ValidationError> result)
        {
            if (listing.IntentionOfOwner == IntentionOfOwner.ForSale)
            {
                if (listing.SaleConditions == null)
                    result.Add(new ValidationError(PropertyValidationErrors.SaleConditionsIsNull));
                else
                {
                    if (!listing.SaleConditions.PriceSpecificationType.HasValue)
                        result.Add(new ValidationError("SaleConditions.PriceSpecificationType",
                            GeneralValidationErrors.ValueNotSpecified));
                    else
                    {
                        if (listing.SaleConditions.PriceSpecificationType.Value == SalePriceSpecificationType.Total &&
                            !listing.SaleConditions.Price.HasValue)
                            result.Add(new ValidationError("SaleConditions.Price",
                                GeneralValidationErrors.ValueNotSpecified));

                        if (listing.SaleConditions.PriceSpecificationType.Value ==
                            SalePriceSpecificationType.PerEstateArea &&
                            !listing.SaleConditions.PricePerEstateArea.HasValue)
                            result.Add(new ValidationError("SaleConditions.PricePerEstateArea",
                                GeneralValidationErrors.ValueNotSpecified));

                        if (listing.SaleConditions.PriceSpecificationType.Value ==
                            SalePriceSpecificationType.PerUnitArea && !listing.SaleConditions.PricePerUnitArea.HasValue)
                            result.Add(new ValidationError("SaleConditions.PricePerUnitArea",
                                GeneralValidationErrors.ValueNotSpecified));
                    }
                }
            }
        }

        private static void ValidateRentConditionsForPublish(PropertyListingDetails listing,
            List<ValidationError> result)
        {
            if (listing.IntentionOfOwner == IntentionOfOwner.ForRent)
            {
                if (listing.RentConditions == null)
                    result.Add(new ValidationError(PropertyValidationErrors.RentConditionsIsNull));
                else
                {
                    if (!listing.RentConditions.Mortgage.HasValue)
                        result.Add(new ValidationError("RentConditions.Mortgage",
                            GeneralValidationErrors.ValueNotSpecified));

                    if (!listing.RentConditions.Rent.HasValue)
                        result.Add(new ValidationError("RentConditions.Rent", GeneralValidationErrors.ValueNotSpecified));
                }
            }
        }

        private static void ValidateContactInfoForPublish(PropertyListingDetails listing, List<ValidationError> result)
        {
            //TODO: check the returned validation error 
            if (listing.ContactInfo == null && listing.ContactInfoID == null)
                result.Add(new ValidationError(PropertyValidationErrors.RentConditionsIsNull));
            else if (listing.ContactInfo != null)
            {
                if (string.IsNullOrWhiteSpace(listing.ContactInfo.ContactName))
                    result.Add(new ValidationError("ContactInfo.ContactName", GeneralValidationErrors.ValueNotSpecified));

                if (string.IsNullOrWhiteSpace(listing.ContactInfo.ContactPhone1))
                    result.Add(new ValidationError("ContactInfo.ContactPhone1",
                        GeneralValidationErrors.ValueNotSpecified));
            }
        }

        private ValidationResult ValidateContactInfo(PropertyListing listing, Db db)
        {
            // If the user is an operator, the edit is valid and there's no need to check anything else.
            if (ServiceContext.Principal.IsOperator)
                return ValidationResult.Success;

            // If the prevoius contactInfos have not been selected, new contact info will be created and validation succeeds.
            if (listing.ContactInfoID == null)
                return ValidationResult.Success;

            // If the new contact info is not tracked by the DbContext, it means a new entity is to be created and validation succeeds.
            if (db.PropertyListingContactInfosDbSet.Local.All(ci => ci != listing.ContactInfo))
                return ValidationResult.Success;

            // If the ContactInfoID isn't changing from the original value (it is the same as the original value), don't check for validation
            var changes = db.ChangeTracker.Entries<PropertyListing>().SingleOrDefault(pl => pl.Entity == listing);
            var dbPropertyEntry = changes.IfNotNull(c => c.Property(pl => pl.ContactInfoID));
            if (changes != null && !dbPropertyEntry.IsModified)
                return ValidationResult.Success;

            // If the user is not authenticated, it should fail since the user doesn't have access to any other ContactInfo entities
            if (!ServiceContext.Principal.IsAuthenticated)
                return ValidationResult.Failure("ContactInfo", GeneralValidationErrors.RelatedEntityIsNotPermitted);

            var contactInfoIsValid = DbManager.Db.PropertyListings.
                Any(
                    pl =>
                        pl.OwnerUserID == ServiceContext.Principal.CoreIdentity.UserId &&
                        pl.ContactInfoID == listing.ContactInfoID && !pl.DeleteDate.HasValue);

            // Operators are allowed to make any changes
            contactInfoIsValid |= ServiceContext.Principal.IsOperator;

            return contactInfoIsValid
                ? ValidationResult.Success
                : ValidationResult.Failure("ContactInfo", GeneralValidationErrors.RelatedEntityIsNotPermitted);
        }

        private static void ValidateListingPropertiesForPublish(PropertyListingDetails listing,
            List<ValidationError> result)
        {
            if (listing.DeliveryDateSpecificationType.HasValue)
            {
                if (listing.DeliveryDateSpecificationType.Value == DeliveryDateSpecificationType.Absolute &&
                    !listing.AbsoluteDeliveryDate.HasValue)
                    result.Add(new ValidationError("AbsoluteDeliveryDate", GeneralValidationErrors.ValueNotSpecified));

                if (listing.DeliveryDateSpecificationType.Value == DeliveryDateSpecificationType.DaysFromContract &&
                    !listing.DeliveryDaysAfterContract.HasValue)
                    result.Add(new ValidationError("DeliveryDaysAfterContract",
                        GeneralValidationErrors.ValueNotSpecified));
            }
        }

        #endregion
    }
}