using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using JahanJooy.Common.Util.EF;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Ad;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Cms;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Directory;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Messages;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.Property;
using JahanJooy.RealEstate.Core.Impl.Data.Configurations.SavedSearch;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Cms;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Messages;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Data
{
	public class Db : ExtendedDbContext
	{
		#region DbSet properties

        #region General application entities

       

		public IDbSet<ApiUser> ApiUsersDbSet
		{
			get { return Set<ApiUser>(); }
		}

		public IDbSet<ConfigurationDataItem> ConfigurationDataItemsDbSet
		{
			get { return Set<ConfigurationDataItem>(); }
		}

		public IDbSet<ScheduledTask> ScheduledTasksDbSet
		{
			get { return Set<ScheduledTask>(); }
		}

		public IDbSet<User> UsersDbSet
        {
            get { return Set<User>(); }
        }

        public IDbSet<UserContactMethod> UserContactMethodsDbSet
        {
            get { return Set<UserContactMethod>(); }
        }

        public IDbSet<Vicinity> VicinityDbSet
        {
            get { return Set<Vicinity>(); }
        }

        #endregion

        #region Ad entities

        public IDbSet<SponsoredEntity> SponsoredEntitiesDbSet
        {
            get { return Set<SponsoredEntity>(); }
        }

        public IDbSet<SponsoredEntityClick> SponsoredEntityClicksDbSet
        {
            get { return Set<SponsoredEntityClick>(); }
        }

        public IDbSet<SponsoredEntityClickBilling> SponsoredEntityClickBillingsDbSet
        {
            get { return Set<SponsoredEntityClickBilling>(); }
        }

        public IDbSet<SponsoredEntityImpression> SponsoredEntityImpressionsDbSet
        {
            get { return Set<SponsoredEntityImpression>(); }
        }

        public IDbSet<SponsoredEntityImpressionBilling> SponsoredEntityImpressionBillingsDbSet
        {
            get { return Set<SponsoredEntityImpressionBilling>(); }
        }

        #endregion

        #region Audit entities

        public IDbSet<AbuseFlag> AbuseFlagsDbSet
        {
            get { return Set<AbuseFlag>(); }
        }

        public IDbSet<ActivityLog> ActivityLogsDbSet
        {
            get { return Set<ActivityLog>(); }
        }

        public IDbSet<HttpSession> HttpSessionsDbSet
        {
            get { return Set<HttpSession>(); }
        }

        public IDbSet<LoginNameQuery> LoginNameQueriesDbSet
        {
            get { return Set<LoginNameQuery>(); }
        }

        public IDbSet<PasswordResetRequest> PasswordResetRequestsDbSet
        {
            get { return Set<PasswordResetRequest>(); }
        }

        public IDbSet<PropertySearchHistory> PropertySearchHistoriesDbSet
        {
            get { return Set<PropertySearchHistory>(); }
        }

        public IDbSet<UniqueVisitor> UniqueVisitorsDbSet
        {
            get { return Set<UniqueVisitor>(); }
        }

        public IDbSet<UserContactMethodVerification> UserContactMethodVerificationsDbSet
        {
            get { return Set<UserContactMethodVerification>(); }
        }

        #endregion

        #region Billing entities

        public IDbSet<PromotionalBonus> PromotionalBonusesDbSet
        {
            get { return Set<PromotionalBonus>(); }
        }

        public IDbSet<PromotionalBonusCoupon> PromotionalBonusCouponsDbSet
        {
            get { return Set<PromotionalBonusCoupon>(); }
        }

        public IDbSet<UserBalanceAdministrativeChange> UserBalanceAdministrativeChangesDbSet
        {
            get { return Set<UserBalanceAdministrativeChange>(); }
        }

        public IDbSet<UserBillingTransaction> UserBillingTransactionsDbSet
        {
            get { return Set<UserBillingTransaction>(); }
        }

        public IDbSet<UserElectronicPayment> UserElectronicPaymentsDbSet
        {
            get { return Set<UserElectronicPayment>(); }
        }

        public IDbSet<UserRefundRequest> UserRefundRequestsDbSet
        {
            get { return Set<UserRefundRequest>(); }
        }

        public IDbSet<UserWireTransferPayment> UserWireTransferPaymentsDbSet
        {
            get { return Set<UserWireTransferPayment>(); }
        }

        #endregion

        #region Cms entities

        public IDbSet<ArticleCategory> ArticleCategoriesDbSet
        {
            get { return Set<ArticleCategory>(); }
        }

        public IDbSet<Article> ArticlesDbSet
        {
            get { return Set<Article>(); }
        }

        public IDbSet<ArticleRevision> ArticleRevisionsDbSet
        {
            get { return Set<ArticleRevision>(); }
        }

        public IDbSet<ArticleSet> ArticleSetsDbSet
        {
            get { return Set<ArticleSet>(); }
        }

        #endregion

        #region Directory entities

        public IDbSet<Agency> AgenciesDbSet
        {
            get { return Set<Agency>(); }
        }

        public IDbSet<AgencyBranch> AgencyBranchesDbSet
        {
            get { return Set<AgencyBranch>(); }
        }

        #endregion

        #region Message related entities

        public IDbSet<OutgoingSmsMessage> OutgoingSmsMessagesDbSet
        {
            get { return Set<OutgoingSmsMessage>(); }
        }

        public IDbSet<NotificationMessage> NotificationMessagesDbSet
        {
			get { return Set<NotificationMessage>(); }
        }

        #endregion

        #region Property listing related entities

        public IDbSet<Building> BuildingsDbSet
        {
            get { return Set<Building>(); }
        }

        public IDbSet<PropertyListingContactInfo> PropertyListingContactInfosDbSet
        {
            get { return Set<PropertyListingContactInfo>(); }
        }

        public IDbSet<Estate> EstatesDbSet
        {
            get { return Set<Estate>(); }
        }

        public IDbSet<PropertyListing> PropertyListingsDbSet
        {
            get { return Set<PropertyListing>(); }
        }

        public IDbSet<PropertyListingPhoto> PropertyListingPhotosDbSet
        {
            get { return Set<PropertyListingPhoto>(); }
        }

        public IDbSet<PropertyListingPublishHistory> PropertyListingPublishHistoriesDbSet
        {
            get { return Set<PropertyListingPublishHistory>(); }
        }

        public IDbSet<PropertyListingUpdateHistory> PropertyListingUpdateHistoriesDbSet
        {
            get { return Set<PropertyListingUpdateHistory>(); }
        }

		public IDbSet<PropertyRequest> PropertyRequestsDbSet
		{
			get { return Set<PropertyRequest>(); }
		}

		public IDbSet<RentConditions> RentConditionsDbSet
        {
            get { return Set<RentConditions>(); }
        }

        public IDbSet<SaleConditions> SaleConditionsDbSet
        {
            get { return Set<SaleConditions>(); }
        }

	    public IDbSet<SponsoredPropertyListing> SponsoredPropertyListingsDbSet
	    {
            get { return Set<SponsoredPropertyListing>(); }
	    }

        public IDbSet<Unit> UnitsDbSet
        {
            get { return Set<Unit>(); }
        }

        public IDbSet<UserFavoritePropertyListing> UserFavoritePropertyListingsDbSet
        {
            get { return Set<UserFavoritePropertyListing>(); }
        }

        #endregion

        #region Saved search entities

        public IDbSet<SavedPropertySearch> SavedPropertySearchesDbSet
		{
			get { return Set<SavedPropertySearch>(); }
		}

		public IDbSet<SavedPropertySearchEmailNotification> SavedPropertySearchEmailNotificationsDbSet
		{
			get { return Set<SavedPropertySearchEmailNotification>(); }
		}

		public IDbSet<SavedPropertySearchGeographicRegion> SavedPropertySearchGeographicRegionsDbSet
		{
			get { return Set<SavedPropertySearchGeographicRegion>(); }
		}

        public IDbSet<SavedPropertySearchPromotionalSms> SavedPropertySearchPromotionalSmsesDbSet
        {
            get { return Set<SavedPropertySearchPromotionalSms>(); }
        }

        public IDbSet<SavedPropertySearchPromotionalSmsNotDeliveredReturn> SavedPropertySearchPromotionalSmsNotDeliveredReturnsDbSet
        {
            get { return Set<SavedPropertySearchPromotionalSmsNotDeliveredReturn>(); }
        }

        public IDbSet<SavedPropertySearchSmsNotification> SavedPropertySearchSmsNotificationsDbSet
        {
            get { return Set<SavedPropertySearchSmsNotification>(); }
        }

        public IDbSet<SavedPropertySearchUpdateHistory> SavedPropertySearchUpdateHistoriesDSet
        {
            get { return Set<SavedPropertySearchUpdateHistory>(); }
        }

        public IDbSet<SavedSearchSmsNotificationBilling> SavedSearchSmsNotificationBillingsDbSet
        {
            get { return Set<SavedSearchSmsNotificationBilling>(); }
        }

        #endregion

        #endregion

        #region NoTracking wrappers

        #region General application entities

       

		public IQueryable<ApiUser> ApiUsers
		{
			get { return ApiUsersDbSet.AsNoTracking(); }
		}
		
		public IQueryable<ConfigurationDataItem> ConfigurationDataItems
		{
			get { return ConfigurationDataItemsDbSet.AsNoTracking(); }
		}

		public IQueryable<ScheduledTask> ScheduledTasks
		{
			get { return ScheduledTasksDbSet.AsNoTracking(); }
		}

		public IQueryable<User> Users
        {
            get { return UsersDbSet.AsNoTracking(); }
        }

        public IQueryable<UserContactMethod> UserContactMethods
        {
            get { return UserContactMethodsDbSet.AsNoTracking().Where(cm => !cm.IsDeleted); }
        }

        public IQueryable<Vicinity> Vicinities
        {
            get { return VicinityDbSet.AsNoTracking(); }
        }

        #endregion

        #region Ad entities

        public IQueryable<SponsoredEntity> SponsoredEntities
        {
            get { return SponsoredEntitiesDbSet.AsNoTracking(); }
        }

        public IQueryable<SponsoredEntityClick> SponsoredEntityClicks
        {
            get { return SponsoredEntityClicksDbSet.AsNoTracking(); }
        }

        public IQueryable<SponsoredEntityClickBilling> SponsoredEntityClickBillings
        {
            get { return SponsoredEntityClickBillingsDbSet.AsNoTracking(); }
        }

        public IQueryable<SponsoredEntityImpression> SponsoredEntityImpressions
        {
            get { return SponsoredEntityImpressionsDbSet.AsNoTracking(); }
        }

        public IQueryable<SponsoredEntityImpressionBilling> SponsoredEntityImpressionBillings
        {
            get { return SponsoredEntityImpressionBillingsDbSet.AsNoTracking(); }
        }

        #endregion

        #region Audit entities

        public IQueryable<AbuseFlag> AbuseFlags
	    {
	        get { return AbuseFlagsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<ActivityLog> ActivityLogs
        {
            get { return ActivityLogsDbSet.AsNoTracking(); }
        }

	    public IQueryable<HttpSession> HttpSessions
        {
            get { return HttpSessionsDbSet.AsNoTracking(); }
        }

	    public IQueryable<LoginNameQuery> LoginNameQueries
	    {
	        get { return LoginNameQueriesDbSet.AsNoTracking(); }
	    }

	    public IQueryable<PasswordResetRequest> PasswordResetRequests
	    {
	        get { return PasswordResetRequestsDbSet.AsNoTracking(); }
	    }

        public IQueryable<PropertySearchHistory> PropertySearchHistories
        {
            get { return PropertySearchHistoriesDbSet.AsNoTracking(); }
        }

        public IQueryable<UniqueVisitor> UniqueVisitors
	    {
	        get { return UniqueVisitorsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<UserContactMethodVerification> UserContactMethodVerifications
	    {
	        get { return UserContactMethodVerificationsDbSet.AsNoTracking(); }
	    }

        #endregion

        #region Billing entities

        public IQueryable<PromotionalBonus> PromotionalBonuses
        {
            get { return PromotionalBonusesDbSet.AsNoTracking(); }
        }

        public IQueryable<PromotionalBonusCoupon> PromotionalBonusCoupons
        {
            get { return PromotionalBonusCouponsDbSet.AsNoTracking(); }
        }

        public IQueryable<UserBalanceAdministrativeChange> UserBalanceAdministrativeChanges
        {
            get { return UserBalanceAdministrativeChangesDbSet.AsNoTracking(); }
        }

        public IQueryable<UserBillingTransaction> UserBillingTransactions
        {
            get { return UserBillingTransactionsDbSet.AsNoTracking(); }
        }

        public IQueryable<UserElectronicPayment> UserElectronicPayments
        {
            get { return UserElectronicPaymentsDbSet.AsNoTracking(); }
        }

        public IQueryable<UserRefundRequest> UserRefundRequests
        {
            get { return UserRefundRequestsDbSet.AsNoTracking(); }
        }

        public IQueryable<UserWireTransferPayment> UserWireTransferPayments
        {
            get { return UserWireTransferPaymentsDbSet.AsNoTracking(); }
        }

        #endregion

        #region Cms entities

        public IQueryable<ArticleCategory> ArticleCategories
        {
            get { return ArticleCategoriesDbSet.AsNoTracking(); }
        }

        public IQueryable<Article> Articles
        {
            get { return ArticlesDbSet.AsNoTracking(); }
        }

        public IQueryable<ArticleRevision> ArticleRevisions
        {
            get { return ArticleRevisionsDbSet.AsNoTracking(); }
        }

        public IQueryable<ArticleSet> ArticleSets
        {
            get { return ArticleSetsDbSet.AsNoTracking(); }
        }

        #endregion

        #region Directory entities

        public IQueryable<Agency> Agencies
        {
            get { return AgenciesDbSet.AsNoTracking().Where(a => !a.DeleteTime.HasValue); }
        }

        public IQueryable<AgencyBranch> AgencyBranches
        {
            get { return AgencyBranchesDbSet.AsNoTracking().Where(ab => !ab.DeleteTime.HasValue); }
        }

        #endregion

        #region Message related entities

        public IQueryable<OutgoingSmsMessage> OutgoingSmsMessages
        {
            get { return OutgoingSmsMessagesDbSet.AsNoTracking(); }
        }

		public IQueryable<NotificationMessage> NotificationMessages
        {
			get { return NotificationMessagesDbSet.AsNoTracking(); }
        }

        #endregion

        #region Property listing related entities

        public IQueryable<Building> Buildings
	    {
	        get { return BuildingsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<PropertyListingContactInfo> PropertyListingContactInfos
	    {
	        get { return PropertyListingContactInfosDbSet.AsNoTracking(); }
	    }

	    public IQueryable<Estate> Estates
        {
            get { return EstatesDbSet.AsNoTracking(); }
        }

	    public IQueryable<PropertyListing> PropertyListings
	    {
	        get { return PropertyListingsDbSet.AsNoTracking().Where(l => !l.DeleteDate.HasValue); }
	    }

	    public IQueryable<PropertyListingPhoto> PropertyListingPhotos
	    {
	        get { return PropertyListingPhotosDbSet.AsNoTracking().Where(plp => plp.DeleteTime == null); }
	    }

	    public IQueryable<PropertyListingPublishHistory> PropertyListingPublishHistories
	    {
	        get { return PropertyListingPublishHistoriesDbSet.AsNoTracking(); }
	    }

	    public IQueryable<PropertyListingUpdateHistory> PropertyListingUpdateHistories
	    {
	        get { return PropertyListingUpdateHistoriesDbSet.AsNoTracking(); }
	    }

		public IQueryable<PropertyRequest> PropertyRequests
		{
			get { return PropertyRequestsDbSet.AsNoTracking(); }
		}

		public IQueryable<RentConditions> RentConditions
        {
            get { return RentConditionsDbSet.AsNoTracking(); }
        }

	    public IQueryable<SaleConditions> SaleConditions
        {
            get { return SaleConditionsDbSet.AsNoTracking(); }
        }

	    public IQueryable<SponsoredPropertyListing> SponsoredPropertyListings
	    {
            get { return SponsoredPropertyListingsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<Unit> Units
	    {
	        get { return UnitsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<UserFavoritePropertyListing> UserFavoritePropertyListings
        {
            get { return UserFavoritePropertyListingsDbSet.AsNoTracking(); }
        }

        #endregion

        #region Saved search entities

        public IQueryable<SavedPropertySearch> SavedPropertySearches
		{
			get { return SavedPropertySearchesDbSet.AsNoTracking().Where(sps => !sps.DeleteTime.HasValue); }
		}

		public IQueryable<SavedPropertySearchEmailNotification> SavedPropertySearchEmailNotifications
		{
			get { return SavedPropertySearchEmailNotificationsDbSet.AsNoTracking(); }
		}

		public IQueryable<SavedPropertySearchGeographicRegion> SavedPropertySearchGeographicRegions
		{
			get { return SavedPropertySearchGeographicRegionsDbSet.AsNoTracking(); }
		}

        public IQueryable<SavedPropertySearchPromotionalSms> SavedPropertySearchPromotionalSmses
	    {
            get { return SavedPropertySearchPromotionalSmsesDbSet.AsNoTracking(); }
	    }

        public IQueryable<SavedPropertySearchPromotionalSmsNotDeliveredReturn> SavedPropertySearchPromotionalSmsNotDeliveredReturns
	    {
            get { return SavedPropertySearchPromotionalSmsNotDeliveredReturnsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<SavedPropertySearchSmsNotification> SavedPropertySearchSmsNotifications
	    {
	        get { return SavedPropertySearchSmsNotificationsDbSet.AsNoTracking(); }
	    }

	    public IQueryable<SavedPropertySearchUpdateHistory> SavedPropertySearchUpdateHistories
	    {
	        get { return SavedPropertySearchUpdateHistoriesDSet.AsNoTracking(); }
	    }

	    public IQueryable<SavedSearchSmsNotificationBilling> SavedSearchSmsNotificationBillings
	    {
            get { return SavedSearchSmsNotificationBillingsDbSet.AsNoTracking(); }
	    }

        #endregion

        #endregion

        public new int SaveChanges()
		{
			throw new InvalidOperationException("Should not call SaveChanges directly in the application code.");
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ScheduledTaskConfiguration());
            modelBuilder.Configurations.Add(new ConfigurationDataItemConfiguration());

            modelBuilder.Configurations.Add(new ApiUserConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
			modelBuilder.Configurations.Add(new UserContactMethodConfiguration());

            modelBuilder.Configurations.Add(new SponsoredEntityClickBillingConfiguration());
            modelBuilder.Configurations.Add(new SponsoredEntityClickConfiguration());
            modelBuilder.Configurations.Add(new SponsoredEntityConfiguration());
            modelBuilder.Configurations.Add(new SponsoredEntityImpressionBillingConfiguration());
            modelBuilder.Configurations.Add(new SponsoredEntityImpressionConfiguration());

            modelBuilder.Configurations.Add(new AbuseFlagConfiguration());
            modelBuilder.Configurations.Add(new ActivityLogConfiguration());
			modelBuilder.Configurations.Add(new HttpSessionConfiguration());
            modelBuilder.Configurations.Add(new LoginNameQueryConfiguration());
            modelBuilder.Configurations.Add(new PasswordResetRequestConfiguration());
            modelBuilder.Configurations.Add(new PropertySearchHistoryConfiguration());
            modelBuilder.Configurations.Add(new UniqueVisitorConfiguration());
			modelBuilder.Configurations.Add(new UserContactMethodVerificationConfiguration());

		    modelBuilder.Configurations.Add(new PromotionalBonusConfiguration());
		    modelBuilder.Configurations.Add(new PromotionalBonusCouponConfiguration());
		    modelBuilder.Configurations.Add(new UserBalanceAdministrativeChangeConfiguration());
		    modelBuilder.Configurations.Add(new UserBillingTransactionConfiguration());
		    modelBuilder.Configurations.Add(new UserElectronicPaymentConfiguration());
		    modelBuilder.Configurations.Add(new UserRefundRequestConfiguration());
		    modelBuilder.Configurations.Add(new UserWireTransferPaymentConfiguration());

		    modelBuilder.Configurations.Add(new ArticleCategoryConfiguration());
		    modelBuilder.Configurations.Add(new ArticleConfiguration());
		    modelBuilder.Configurations.Add(new ArticleRevisionConfiguration());
		    modelBuilder.Configurations.Add(new ArticleSetConfiguration());

            modelBuilder.Configurations.Add(new AgencyConfiguration());
            modelBuilder.Configurations.Add(new AgencyBranchConfiguration());
            
            modelBuilder.Configurations.Add(new OutgoingSmsMessageConfiguration());
            modelBuilder.Configurations.Add(new NotificationMessageConfiguration());

            modelBuilder.Configurations.Add(new BuildingConfiguration());
            modelBuilder.Configurations.Add(new PropertyListingContactInfoConfiguration());
            modelBuilder.Configurations.Add(new EstateConfiguration());
			modelBuilder.Configurations.Add(new PropertyListingConfiguration());
			modelBuilder.Configurations.Add(new PropertyListingPhotoConfiguration());
            modelBuilder.Configurations.Add(new PropertyListingPublishHistoryConfiguration());
            modelBuilder.Configurations.Add(new PropertyListingUpdateHistoryConfiguration());
            modelBuilder.Configurations.Add(new PropertyRequestConfiguration());
            modelBuilder.Configurations.Add(new RentConditionsConfiguration());
            modelBuilder.Configurations.Add(new SaleConditionsConfiguration());
		    modelBuilder.Configurations.Add(new SponsoredPropertyListingConfiguration());
			modelBuilder.Configurations.Add(new UnitConfiguration());
			modelBuilder.Configurations.Add(new UserFavoritePropertyListingConfiguration());

			modelBuilder.Configurations.Add(new SavedPropertySearchConfiguration());
            modelBuilder.Configurations.Add(new SavedPropertySearchEmailNotificationConfiguration());
            modelBuilder.Configurations.Add(new SavedPropertySearchGeographicRegionConfiguration());
			modelBuilder.Configurations.Add(new SavedPropertySearchPromotionalSmsConfiguration());
			modelBuilder.Configurations.Add(new SavedPropertySearchPromotionalSmsNotDeliveredReturnConfiguration());
			modelBuilder.Configurations.Add(new SavedPropertySearchSmsNotificationConfiguration());
			modelBuilder.Configurations.Add(new SavedPropertySearchUpdateHistoryConfiguration());
			modelBuilder.Configurations.Add(new SavedSearchSmsNotificationBillingConfiguration());
		}
	}
}