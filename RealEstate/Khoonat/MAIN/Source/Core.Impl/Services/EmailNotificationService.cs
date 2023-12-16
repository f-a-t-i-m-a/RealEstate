using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Services.Resources;
using JahanJooy.RealEstate.Core.Impl.Templates;
using JahanJooy.RealEstate.Core.Impl.Templates.Email;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Domain.SavedSearch;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Presentation.Property;
using log4net;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class EmailNotificationService : IEmailNotificationService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EmailNotificationService));

        #region Component plugs

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IEmailTransmitter EmailTransmitter { get; set; }

        [ComponentPlug]
        public RazorTemplateRunner TemplateRunner { get; set; }

        [ComponentPlug]
        public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

        [ComponentPlug]
        public IUserBillingBalanceCache UserBillingBalanceCache { get; set; }

        #endregion

        #region Template instances

        private readonly IRazorTemplate _contactMethodVerificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.ContactMethodVerification.cshtml");

        private readonly IRazorTemplate _passwordResetTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PasswordReset.cshtml");

        private readonly IRazorTemplate _loginNameQueryTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.LoginNameQuery.cshtml");

        private readonly IRazorTemplate _passwordRecoveryBySmsStartNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PasswordRecoveryBySmsStartNotification.cshtml");

        private readonly IRazorTemplate _passwordRecoveryByUnregisteredEmailNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PasswordRecoveryByUnregisteredEmailNotification.cshtml");

        private readonly IRazorTemplate _passwordRecoveryByUnverifiedEmailNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PasswordRecoveryByUnverifiedEmailNotification.cshtml");

        private readonly IRazorTemplate _passwordRecoveryCompletionNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PasswordRecoveryCompletionNotification.cshtml");

        private readonly IRazorTemplate _accountDisabledNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.AccountDisabledNotification.cshtml");

        private readonly IRazorTemplate _propertyInfoForSavedSearchNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PropertyInfoForSavedSearch.cshtml");

        private readonly IRazorTemplate _propertyPublishExpirationWarningTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PropertyPublishExpirationWarning.cshtml");

        private readonly IRazorTemplate _propertyRegistrationInfoForOwnerTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.PropertyRegistrationInfoForOwner.cshtml");

        private readonly IRazorTemplate _bonusNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.NotifyBonusAwarded.cshtml");

        private readonly IRazorTemplate _inactiveUserNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.InactiveUserNotification.cshtml");

        private readonly IRazorTemplate _balanceNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.BalanceNotification.cshtml");

        private readonly IRazorTemplate _wireTransferPaymentNotificationTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.NotifyWireTransferPayment.cshtml");

        private readonly IRazorTemplate _sharePropertyListingTemplate =
            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                "JahanJooy.RealEstate.Core.Impl.Templates.Email.SharePropertyListing.cshtml");

        #endregion

        #region IEmailNotificationService implementation

        public void SendVerification(User user, UserContactMethod contactMethod, UserContactMethodVerification log)
        {
            Log.DebugFormat("SendVerification for user {0}, contact methdod {1}, verification id {2}",
                user.IfNotNull(u => u.ID, -1),
                contactMethod.IfNotNull(cm => cm.ID, -1),
                log.IfNotNull(l => l.ID, -1));

            if (contactMethod.ContactMethodType != ContactMethodType.Email)
                throw new ArgumentException("The contact method is not Email.");

            string subject = EmailNotificationServiceResources.SendVerification_Subject;
            string body = TemplateRunner.ApplyTemplate(_contactMethodVerificationTemplate,
                new ContactMethodVerificationModel {User = user, ContactMethod = contactMethod, Log = log});

            EmailTransmitter.Send(subject, body, new[] {contactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void SendPasswordReset(User user, UserContactMethod contactMethod,
            PasswordResetRequest passwordResetRequest)
        {
            Log.DebugFormat("SendPasswordReset for user {0}, contact methdod {1}, request id {2}",
                user.IfNotNull(u => u.ID, -1),
                contactMethod.IfNotNull(cm => cm.ID, -1),
                passwordResetRequest.IfNotNull(prr => prr.ID, -1));

            if (passwordResetRequest.ContactMethod == null)
                throw new ArgumentException("ContactMethod is not filled for passwordResetRequest parameter.");
            if (passwordResetRequest.ContactMethod.ContactMethodType != ContactMethodType.Email)
                throw new ArgumentException("The password reset target method is not Email");

            string subject = EmailNotificationServiceResources.PasswordReset_Subject;
            string body = TemplateRunner.ApplyTemplate(_passwordResetTemplate,
                new PasswordResetModel {User = user, ContactMethod = contactMethod, Request = passwordResetRequest});

            EmailTransmitter.Send(subject, body, new[] {passwordResetRequest.ContactMethod.ContactMethodText},
                Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public void SendLoginNameQuery(User user, UserContactMethod contactMethod)
        {
            Log.DebugFormat("SendLoginNameQuery for user {0}, contact methdod {1}",
                user.IfNotNull(u => u.ID, -1),
                contactMethod.IfNotNull(cm => cm.ID, -1));

            if (contactMethod.ContactMethodType != ContactMethodType.Email)
                throw new ArgumentException("The contact method is not Email.");

            string subject = EmailNotificationServiceResources.LoginNameQuery_Subject;
            string body = TemplateRunner.ApplyTemplate(_loginNameQueryTemplate,
                new LoginNameQueryModel {User = user, ContactMethod = contactMethod});

            EmailTransmitter.Send(subject, body, new[] {contactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyPasswordRecoveryBySmsStarted(User targetUser, UserContactMethod contactMethod,
            PasswordResetRequest passwordResetRequest)
        {
            Log.DebugFormat("NotifyPasswordRecoveryBySmsStarted for user {0}, contact methdod {1}, request id {2}",
                targetUser.IfNotNull(u => u.ID, -1),
                contactMethod.IfNotNull(cm => cm.ID, -1),
                passwordResetRequest.IfNotNull(prr => prr.ID, -1));

            var verifiedEmails = LoadVerifiedEmails(targetUser.ID);
            if (!verifiedEmails.Any())
                return;

            string subject = EmailNotificationServiceResources.NotifyPasswordRecoveryBySmsStarted_Subject;
            string body = TemplateRunner.ApplyTemplate(_passwordRecoveryBySmsStartNotificationTemplate,
                new PasswordRecoveryBySmsStartNotificationModel
                {
                    User = targetUser,
                    ContactMethod = contactMethod,
                    Request = passwordResetRequest
                });

            EmailTransmitter.Send(subject, body, verifiedEmails, Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public void NotifyNonRegisteredEmailUsedForPasswordRecovery(string targetEmail, string requestedLoginName)
        {
            Log.DebugFormat("NotifyNonRegisteredEmailUsedForPasswordRecovery for email {0}, login name {1}",
                targetEmail ?? "<NULL>",
                requestedLoginName ?? "<NULL>");

            string subject = EmailNotificationServiceResources.NotifyNonRegisteredEmailUsedForPasswordRecovery_Subject;
            string body = TemplateRunner.ApplyTemplate(_passwordRecoveryByUnregisteredEmailNotificationTemplate,
                new PasswordRecoveryByUnregisteredEmailNotificationModel
                {
                    TargetEmail = targetEmail,
                    RequestedLoginName = requestedLoginName
                });

            EmailTransmitter.Send(subject, body, new[] {targetEmail}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyNonVerifiedEmailUsedForPasswordRecovery(string targetEmail, string requestedLoginName)
        {
            Log.DebugFormat("NotifyNonVerifiedEmailUsedForPasswordRecovery for email {0}, login name {1}",
                targetEmail ?? "<NULL>",
                requestedLoginName ?? "<NULL>");

            string subject = EmailNotificationServiceResources.NotifyNonVerifiedEmailUsedForPasswordRecovery_Subject;
            string body = TemplateRunner.ApplyTemplate(_passwordRecoveryByUnverifiedEmailNotificationTemplate,
                new PasswordRecoveryByUnverifiedEmailNotificationModel
                {
                    TargetEmail = targetEmail,
                    RequestedLoginName = requestedLoginName
                });

            EmailTransmitter.Send(subject, body, new[] {targetEmail}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyPasswordRecoveryComplete(User user, PasswordResetRequest passwordResetRequest)
        {
            Log.DebugFormat("NotifyPasswordRecoveryComplete for user {0}, request id {1}",
                user.IfNotNull(u => u.ID, -1),
                passwordResetRequest.IfNotNull(prr => prr.ID, -1));

            var verifiedEmails = LoadVerifiedEmails(user.ID);
            if (!verifiedEmails.Any())
                return;

            var contactMethod =
                DbManager.Db.UserContactMethods.SingleOrDefault(cm => cm.ID == passwordResetRequest.ContactMethodID);
            if (contactMethod == null)
                return;

            string subject = EmailNotificationServiceResources.NotifyPasswordRecoveryComplete_Subject;
            string body = TemplateRunner.ApplyTemplate(_passwordRecoveryCompletionNotificationTemplate,
                new PasswordRecoveryCompletionNotificationModel
                {
                    User = user,
                    ContactMethod = contactMethod,
                    Request = passwordResetRequest
                });

            EmailTransmitter.Send(subject, body, verifiedEmails, Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public void NotifyAccountHasBeenDisabled(User user)
        {
            Log.DebugFormat("NotifyAccountHasBeenDisabled for user {0}",
                user.IfNotNull(u => u.ID, -1));

            var verifiedEmails = LoadVerifiedEmails(user.ID);
            if (!verifiedEmails.Any())
                return;

            string subject = EmailNotificationServiceResources.NotifyAccountHasBeenDisabled_Subject;
            string body = TemplateRunner.ApplyTemplate(_accountDisabledNotificationTemplate,
                new AccountDisabledNotificationModel {User = user});

            EmailTransmitter.Send(subject, body, verifiedEmails, Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public void NotifyPublishAboutToExpire(long listingId)
        {
            Log.DebugFormat("NotifyPublishAboutToExpire for listing ID {0}", listingId);

            var listing = DbManager.Db.PropertyListings
                .IncludeInfoProperties()
                .Include(l => l.ContactInfo)
                .Include(l => l.OwnerUser)
                .Include(l => l.OwnerUser.ContactMethods)
                .SingleOrDefault(l => l.ID == listingId);

            if (listing == null)
                return;

            var listingDetails = PropertyListingDetails.MakeDetails(listing);
            var listingSummary = PropertyListingSummary.Summarize(listingDetails);

            string targetEmailAddress = null;
            var subject = EmailNotificationServiceResources.NotifyPublishAboutToExpire_Subject;
            var model = new PropertyPublishExpirationWarningModel {Listing = listing, ListingSummary = listingSummary};

            if (listing.OwnerUser != null && listing.OwnerUser.ContactMethods != null)
            {
                var contactMethod =
                    listing.OwnerUser.ContactMethods.FirstOrDefault(
                        cm => cm.ContactMethodType == ContactMethodType.Email && cm.IsVerified && !cm.IsDeleted);
                if (contactMethod != null)
                {
                    model.OwnerName = listing.OwnerUser.FullName;
                    targetEmailAddress = contactMethod.ContactMethodText;
                }
            }

            if (targetEmailAddress == null && listing.ContactInfo != null &&
                !string.IsNullOrWhiteSpace(listing.ContactInfo.ContactEmail))
            {
                model.OwnerName = listing.ContactInfo.ContactName;
                targetEmailAddress = listing.ContactInfo.ContactEmail;
            }

            if (!string.IsNullOrEmpty(targetEmailAddress) && EmailUtils.IsValidEmail(targetEmailAddress))
            {
                string body = TemplateRunner.ApplyTemplate(_propertyPublishExpirationWarningTemplate, model);
                EmailTransmitter.Send(subject, body, new[] {targetEmailAddress}, Enumerable.Empty<string>(),
                    Enumerable.Empty<string>());
            }
        }

        public void SendPropertyInfoForSavedSearch(User user, PropertyListingDetails listing,
            PropertyListingSummary listingSummary, SavedPropertySearch savedSearch)
        {
            Log.DebugFormat("SendPropertyInfoForSavedSearch for user {0}, listing {1}, saved search {2}",
                user.IfNotNull(u => u.ID, -1),
                listing.IfNotNull(l => l.ID, -1),
                savedSearch.IfNotNull(ss => ss.ID, -1));

            if (savedSearch.EmailNotificationTarget == null)
                throw new ArgumentException("No email notification target specified");

            if (savedSearch.EmailNotificationTarget.ContactMethodType != ContactMethodType.Email)
                throw new ArgumentException(
                    "Invalid target type: EmailNotificationTarget does not point to an email contact method.");

            string subject = string.Format(EmailNotificationServiceResources.PropertyInfoForSavedSearch_Subject,
                PropertyPresentationHelper.BuildPageTitleString(listingSummary));
            string body = TemplateRunner.ApplyTemplate(_propertyInfoForSavedSearchNotificationTemplate,
                new PropertyInfoForSavedSearchModel
                {
                    PropertyPresentationHelper = Composer.GetComponent<PropertyPresentationHelper>(),
                    User = user,
                    Listing = listing,
                    ListingSummary = listingSummary,
                    SavedSearch = savedSearch
                });

            EmailTransmitter.Send(subject, body, new[] {savedSearch.EmailNotificationTarget.ContactMethodText},
                Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public void SendPropertyRegistrationInfoToOwner(PropertyListingContactInfo contactInfo,
            PropertyListingSummary listingSummary, string listingEditPassword)
        {
            Log.DebugFormat(
                "SendPropertyRegistrationInfoToOwner for property listing {0}, property listing contact info {1}",
                listingSummary.IfNotNull(l => l.ID, -1),
                contactInfo.IfNotNull(ci => ci.ID, -1));

            if (contactInfo == null ||
                contactInfo.ContactEmail == null ||
                string.IsNullOrWhiteSpace(contactInfo.ContactEmail) ||
                !EmailUtils.IsValidEmail(contactInfo.ContactEmail))
            {
                return;
            }

            string subject = EmailNotificationServiceResources.SendPropertyRegistrationInfoToOwner_Subject;

            string body = TemplateRunner.ApplyTemplate(_propertyRegistrationInfoForOwnerTemplate,
                new PropertyRegistrationInfoForOwnerModel
                {
                    PropertyPresentationHelper = Composer.GetComponent<PropertyPresentationHelper>(),
                    ListingSummary = listingSummary,
                    ContactInfo = contactInfo,
                    ListingEditPassword = listingEditPassword
                });

            EmailTransmitter.Send(subject, body, new[] {contactInfo.ContactEmail}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyBonusAwarded(UserContactMethod userContactMethod, PromotionalBonus bonus)
        {
            Log.DebugFormat("NotifyBonusAwarded for user {0}, contact methdod {1}, bonus id {2}",
                userContactMethod.IfNotNull(ucm => ucm.UserID, -1),
                userContactMethod.IfNotNull(ucm => ucm.ID, -1),
                bonus.IfNotNull(b => b.ID, -1));

            if (userContactMethod == null ||
                userContactMethod.ContactMethodType != ContactMethodType.Email ||
                string.IsNullOrWhiteSpace(userContactMethod.ContactMethodText) ||
                !EmailUtils.IsValidEmail(userContactMethod.ContactMethodText))
            {
                throw new ArgumentException("The contact method is not Email.");
            }

            string subject = EmailNotificationServiceResources.BonusInformation_Subject;
            string body = TemplateRunner.ApplyTemplate(_bonusNotificationTemplate,
                new NotifyBonusAwardedModel {User = userContactMethod.User, Bonus = bonus});

            EmailTransmitter.Send(subject, body, new[] {userContactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyInactiveUser(UserContactMethod userContactMethod)
        {
            Log.DebugFormat("NotifyInactiveUser for user {0}, contact methdod {1}",
                userContactMethod.IfNotNull(ucm => ucm.UserID, -1),
                userContactMethod.IfNotNull(ucm => ucm.ID, -1));

            if (userContactMethod == null ||
                userContactMethod.ContactMethodType != ContactMethodType.Email ||
                string.IsNullOrWhiteSpace(userContactMethod.ContactMethodText) ||
                !EmailUtils.IsValidEmail(userContactMethod.ContactMethodText))
            {
                throw new ArgumentException("The contact method is not Email.");
            }

            string subject = EmailNotificationServiceResources.InactiveUser_Subject;
            string body = TemplateRunner.ApplyTemplate(_inactiveUserNotificationTemplate,
                new InactiveUserNotificationModel {User = userContactMethod.User});

            EmailTransmitter.Send(subject, body, new[] {userContactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void SendBalanceNotification(User user, UserContactMethod userContactMethod, bool isBalanceDepleted)
        {
            Log.DebugFormat("SendBalanceNotification for user {0}, contact methdod {1}, depleted {2}",
                user.IfNotNull(u => u.ID, -1),
                userContactMethod.IfNotNull(cm => cm.ID, -1),
                isBalanceDepleted);

            if (userContactMethod == null ||
                userContactMethod.ContactMethodType != ContactMethodType.Email ||
                string.IsNullOrWhiteSpace(userContactMethod.ContactMethodText) ||
                !EmailUtils.IsValidEmail(userContactMethod.ContactMethodText))
            {
                throw new ArgumentException("The contact method is not Email.");
            }

            string subject = isBalanceDepleted
                ? EmailNotificationServiceResources.BalanceDepleted_Subject
                : EmailNotificationServiceResources.BalanceRunningLow_Subject;
            string body = TemplateRunner.ApplyTemplate(_balanceNotificationTemplate,
                new BalanceNotificationModel
                {
                    User = user,
                    IsBalanceDepleted = isBalanceDepleted,
                    UserTotalBalance = UserBillingBalanceCache[user.ID].TotalBalance
                });

            EmailTransmitter.Send(subject, body, new[] {userContactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void NotifyWireTransferPayment(UserContactMethod userContactMethod, UserWireTransferPayment payment)
        {
            Log.DebugFormat("NotifyWireTransferPayment for user {0}, contact methdod {1}, payment id {2}",
                userContactMethod.IfNotNull(ucm => ucm.UserID, -1),
                userContactMethod.IfNotNull(ucm => ucm.ID, -1),
                payment.IfNotNull(p => p.ID, -1));

            if (userContactMethod == null ||
                userContactMethod.ContactMethodType != ContactMethodType.Email ||
                string.IsNullOrWhiteSpace(userContactMethod.ContactMethodText) ||
                !EmailUtils.IsValidEmail(userContactMethod.ContactMethodText))
            {
                throw new ArgumentException("The contact method is not Email.");
            }

            string subject = EmailNotificationServiceResources.NotifyWireTransferPayment_Subject;
            string body = TemplateRunner.ApplyTemplate(_wireTransferPaymentNotificationTemplate,
                new NotifyWireTransferPaymentModel {User = userContactMethod.User, Payment = payment});

            EmailTransmitter.Send(subject, body, new[] {userContactMethod.ContactMethodText}, Enumerable.Empty<string>(),
                Enumerable.Empty<string>());
        }

        public void SharePropertyListing(string email, string receiverName, string description,
            PropertyListingDetails propertyListingDetails, PropertyListingSummary propertyListingSummary)
        {
            Log.InfoFormat("SendPropertyInfo for user {0}, listing {1}",
                receiverName,
                propertyListingDetails.IfNotNull(l => l.ID, -1));

            if (email.IsNullOrEmpty())
                throw new ArgumentException("No email Address specified");

            string subject = string.Format(EmailNotificationServiceResources.SharePropertyListing_Subject,
                PropertyPresentationHelper.BuildPageTitleString(propertyListingSummary));
            string body = TemplateRunner.ApplyTemplate(_sharePropertyListingTemplate,
                new SharePropertyListingModel
                {
                    PropertyPresentationHelper = Composer.GetComponent<PropertyPresentationHelper>(),
                    ReceiverName = receiverName,
                    Description = description,
                    SenderDisplayName = ServiceContext.Principal.CoreIdentity.DisplayName,
                    SenderID = ServiceContext.Principal.CoreIdentity.UserId,
                    Listing = propertyListingDetails,
                    ListingSummary = propertyListingSummary,
                });

            EmailTransmitter.Send(subject, body, new[] {email},
                Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        #endregion

        #region Private helper methods

        private string[] LoadVerifiedEmails(long userId)
        {
            return DbManager.Db
                .UserContactMethods
                .Where(
                    cm =>
                        cm.UserID == userId && !cm.IsDeleted && cm.IsVerified &&
                        cm.ContactMethodType == ContactMethodType.Email)
                .Select(cm => cm.ContactMethodText).ToArray();
        }

        #endregion
    }
}