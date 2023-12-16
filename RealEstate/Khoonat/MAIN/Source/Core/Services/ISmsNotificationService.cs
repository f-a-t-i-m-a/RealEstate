using Compositional.Composer;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface ISmsNotificationService
	{
		void SendVerification(User user, UserContactMethod contactMethod, UserContactMethodVerification verification);
		void SendPasswordReset(User user, PasswordResetRequest passwordResetRequest);
		void SendLoginNameQuery(User user, UserContactMethod contactMethod);
        void NotifyBonusAwarded(UserContactMethod userContactMethod, PromotionalBonus newBonus);
        void NotifyBalanceDepleted(UserContactMethod contactMethod, NotificationMessage notification);
        void NotifyWireTransferPayment(UserContactMethod userContactMethod, UserWireTransferPayment payment);
	}
}