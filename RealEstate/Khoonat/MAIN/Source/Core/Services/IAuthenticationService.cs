using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Authentication;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IAuthenticationService
	{
		AuthenticationResult Authenticate(AuthenticationRequest request);
		void ResetFailedLoginCount(long userId);
		CorePrincipal LoadPrincipal(long userId);
		User LoadCurrentUserInfo();
		User LoadUserInfo(long id);
		User LoadUserInfo(string loginName);

		bool CheckLoginNameExists(string loginName);
		ValidatedResult<CorePrincipal> CreateUser(User user, string password);
		ValidationResult UpdateUser(User user);
		void MarkEnabled(long userId, bool enabled);
		ValidationResult ChangePassword(string currentPassword, string newPassword);
		ValidationResult ResetPassword(long userId, string newPassword);

		ValidatedResult<UserContactMethod> AddContactMethod(long userId, UserContactMethod contactMethod, bool shouldBeVerifyable);
		void DeleteContactMethod(long contactMethodId);
		UserContactMethodVerificationInfo LoadContactMethodVerificationInfo(long contactMethodId);
        UserContactMethodVerificationInfo StartContactMethodVerification(long contactMethodId);
		ValidationResult PerformContactMethodVerification(long contactMethodId, string secret);
		void PerformContactMethodVerificationAdministratively(long contactMethodId);
		void CancelContactMethodVerification(long contactMethodId);

		ValidationResult StartPasswordRecovery(string loginName, ContactMethodType contactMethodType, string target);
		ValidationResult PerformPasswordRecovery(string loginName, string passwordResetToken, string newPassword);
        ValidationResult PerformLoginNameRecovery(ContactMethodType contactMethodType, string target);

		UniqueVisitor LookupVisitor(string uniqueIdentifier);
		UniqueVisitor CreateVisitor();
		
		SessionInfo StartSession(HttpSession initialSessionData);
		void AcknowledgeCurrentSessionAsInteractive();
		void MarkSessionAsAuthenticated(long sessionId, long userId);
		void EndSession(SessionInfo sessionInfo, SessionEndReason reason);

		void CleanupSessionsOnApplicationStartup();
		void CleanupSessionsOnApplicationShutdown();
	}
}