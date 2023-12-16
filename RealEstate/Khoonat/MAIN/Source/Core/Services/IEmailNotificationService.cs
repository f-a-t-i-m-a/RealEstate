using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IEmailNotificationService
	{
		void SendVerification(User user, UserContactMethod contactMethod, UserContactMethodVerification log);
		void SendPasswordReset(User user, UserContactMethod contactMethod, PasswordResetRequest passwordResetRequest);
		void SendLoginNameQuery(User user, UserContactMethod contactMethod);
		void NotifyPasswordRecoveryBySmsStarted(User targetUser, UserContactMethod contactMethod, PasswordResetRequest passwordResetRequest);
		void NotifyNonRegisteredEmailUsedForPasswordRecovery(string targetEmail, string requestedLoginName);
		void NotifyNonVerifiedEmailUsedForPasswordRecovery(string targetEmail, string requestedLoginName);
		void NotifyPasswordRecoveryComplete(User user, PasswordResetRequest lastLog);
		void NotifyAccountHasBeenDisabled(User user);
        void NotifyPublishAboutToExpire(long propertyListingId);
		void SendPropertyInfoForSavedSearch(User user, PropertyListingDetails listing, PropertyListingSummary listingSummary, SavedPropertySearch savedSearch);
        void SendPropertyRegistrationInfoToOwner(PropertyListingContactInfo contactInfo, PropertyListingSummary listingSummary, string listingEditPassword);
        void NotifyBonusAwarded(UserContactMethod userContactMethod, PromotionalBonus newBonus);
        void NotifyInactiveUser(UserContactMethod userContactMethod);
        void SendBalanceNotification(User user, UserContactMethod userContactMethod, bool isBalanceRunningLow);
        void NotifyWireTransferPayment(UserContactMethod userContactMethod, UserWireTransferPayment payment);
        void SharePropertyListing(string email, string receiverName, string description, PropertyListingDetails propertyListingDetails, PropertyListingSummary propertyListingSummary);
	}
}