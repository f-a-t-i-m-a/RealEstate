using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Localization;
using JahanJooy.Common.Util.PhoneNumbers;
using JahanJooy.Common.Util.Security;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Security;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Authentication;
using JahanJooy.RealEstate.Core.Services.Enums;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class AuthenticationService : IAuthenticationService
    {
        private const int LockoutFailedLogins = 5;
        private const int NationalLocaleCode = 98;
        private const int SecretLengthForSms = 6;
        private const int SecretLengthForEmail = 12;

        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(60);
        private static readonly TimeSpan MinimumTimeBetweenTwoPasswordRecoveryAttempts = TimeSpan.FromHours(1);
        private static readonly TimeSpan MinimumTimeBetweenTwoLoginNameRecoveryAttempts = TimeSpan.FromHours(3);
        private static readonly TimeSpan MinimumTimeBetweenTwoContactMethodVerificationStarts = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan ExpirationDelayForVerificationByEmail = TimeSpan.FromDays(1);
        private static readonly TimeSpan ExpirationDelayForVerificationBySms = TimeSpan.FromHours(6);
        private static readonly TimeSpan ExpirationDelayForPasswordReset = TimeSpan.FromHours(1);

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        [ComponentPlug]
        public ISmsNotificationService SmsNotificationService { get; set; }

        [ComponentPlug]
        public IPromotionalBonusService PromotionalBonusService { get; set; }

        #region Implementation of IAuthenticationService

        public AuthenticationResult Authenticate(AuthenticationRequest request)
        {
            var passwordAuthenticationRequest = request as PasswordAuthenticationRequest;
            if (passwordAuthenticationRequest != null)
                return AuthenticatePassword(passwordAuthenticationRequest);

            throw new NotSupportedException("This type of authentication is not implemented yet.");
        }

        public void ResetFailedLoginCount(long userId)
        {
            var user = DbManager.Db.UsersDbSet.SingleOrDefault(u => u.ID == userId);
            if (user == null)
                throw new ArgumentException("Unknown User ID");

            user.FailedLoginAttempts = 0;
        }

        public CorePrincipal LoadPrincipal(long userId)
        {
            var user = DbManager.Db.Users.SingleOrDefault(u => u.ID == userId);
            if (user == null)
                return CorePrincipal.Anonymous;

            return new CorePrincipal(user);
        }

        public User LoadCurrentUserInfo()
        {
            if (ServiceContext.Principal == null || ServiceContext.Principal.CoreIdentity == null ||
                !ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                return null;

            return LoadUserInfo(ServiceContext.Principal.CoreIdentity.UserId.Value);
        }

        public User LoadUserInfo(long id)
        {
            var result = DbManager.Db.Users
                .Include(u => u.ContactMethods)
                .Include(u => u.Agency.AgencyBranches)
                .SingleOrDefault(u => u.ID == id);
            if (result == null)
                return null;

            var currentUserVisibilityLevel = FindCurrentUserVisibilityLevel(id);

            result.PasswordHash = null;
            result.PasswordSalt = null;
            result.ContactMethods =
                result.ContactMethods.Where(cm => cm.Visibility >= currentUserVisibilityLevel && !cm.IsDeleted).ToList();

            return result;
        }

        public User LoadUserInfo(string loginName)
        {
            var result = DbManager.Db.Users
                .Include(u => u.ContactMethods)
                .Include(u => u.Agency.AgencyBranches)
                .SingleOrDefault(u => u.LoginName == loginName);
            if (result == null)
                return null;

            var currentUserVisibilityLevel = FindCurrentUserVisibilityLevel(result.ID);

            result.PasswordHash = null;
            result.PasswordSalt = null;
            result.ContactMethods =
                result.ContactMethods.Where(cm => cm.Visibility >= currentUserVisibilityLevel && !cm.IsDeleted).ToList();

            return result;
        }

        public bool CheckLoginNameExists(string loginName)
        {
            if (string.IsNullOrWhiteSpace(loginName))
                return false;

            loginName = loginName.Trim().ToLowerInvariant();

            return DbManager.Db.Users.Any(u => u.LoginName == loginName);
        }

        public ValidatedResult<CorePrincipal> CreateUser(User user, string password)
        {
            user = BuildUserForCreation(user);
            var errors = new List<ValidationError>();
          
            foreach (var contactMethod in user.ContactMethods)
            {
                var resultText=ValidateContactMethodTextForVerification(contactMethod);
                if (!resultText.IsValid)
                    errors.AddRange(resultText.Errors);

                var resultDuplication = ValidateContactMethodDuplicationForVerification(contactMethod);
                if (!resultDuplication.IsValid)
                    errors.AddRange(resultDuplication.Errors);
            }
           
            
            ValidateUserForCreation(user, errors);
            ValidatePassword(user, password, errors);

            if (errors.Count > 0)
                return new ValidatedResult<CorePrincipal> {Errors = errors};

            PasswordHashUtil.SetSaltAndHash(user, password);

            DbManager.Db.UsersDbSet.Add(user);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Create,
                ActivityLogDetails.UserActionDetails.Register);
            return ValidatedResult<CorePrincipal>.Success(new CorePrincipal(user));
        }

        public ValidationResult UpdateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.DisplayName))
                return ValidationResult.Failure("DisplayName", GeneralValidationErrors.ValueNotSpecified);
            if (string.IsNullOrWhiteSpace(user.FullName))
                return ValidationResult.Failure("FullName", GeneralValidationErrors.ValueNotSpecified);

            User originalUser = DbManager.Db.UsersDbSet.Find(user.ID);
            if (originalUser == null)
                throw new InvalidOperationException("User not found");

            originalUser.DisplayName = user.DisplayName;
            originalUser.FullName = user.FullName;
            originalUser.ModificationDate = DateTime.Now;

            ActivityLogService.ReportActivity(TargetEntityType.User, originalUser.ID, ActivityAction.Edit,
                ActivityLogDetails.UserActionDetails.EditProfile);
            return ValidationResult.Success;
        }

        public void MarkEnabled(long userId, bool enabled)
        {
            User originalUser = DbManager.Db.UsersDbSet.Find(userId);
            if (originalUser == null)
                throw new InvalidOperationException("User not found");

            originalUser.IsEnabled = enabled;
            originalUser.ModificationDate = DateTime.Now;

            ActivityLogService.ReportActivity(TargetEntityType.User, originalUser.ID,
                enabled ? ActivityAction.Enable : ActivityAction.Disable);
        }

        public ValidationResult ChangePassword(string currentPassword, string newPassword)
        {
            if (ServiceContext.Principal == null || ServiceContext.Principal.CoreIdentity == null ||
                !ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                throw new InvalidOperationException("Logged-in user not found in the service context.");

            var user = DbManager.Db.UsersDbSet.Find(ServiceContext.Principal.CoreIdentity.UserId.Value);
            if (user == null)
                throw new InvalidOperationException("Logged in user not found on the database.");

            var errors = new List<ValidationError>();
            ValidatePassword(user, newPassword, errors);
            if (errors.Count > 0)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Edit,
                    ActivityLogDetails.UserActionDetails.ChangePassword, false);
                return new ValidatedResult<CorePrincipal> {Errors = errors};
            }

            if (!PasswordHashUtil.VerifySaltAndHash(user, currentPassword))
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Edit,
                    ActivityLogDetails.UserActionDetails.ChangePassword, false);
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordDoesNotMatch);
            }

            PasswordHashUtil.SetSaltAndHash(user, newPassword);

            ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Edit,
                ActivityLogDetails.UserActionDetails.ChangePassword, true);
            return ValidationResult.Success;
        }

        public ValidationResult ResetPassword(long userId, string newPassword)
        {
            var user = DbManager.Db.UsersDbSet.Find(userId);
            if (user == null)
                throw new InvalidOperationException("Specified user could not be found.");

            var errors = new List<ValidationError>();
            ValidatePassword(user, newPassword, errors);
            if (errors.Count > 0)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Edit,
                    ActivityLogDetails.UserActionDetails.ResetPassword, false);
                return new ValidatedResult<CorePrincipal> {Errors = errors};
            }

            PasswordHashUtil.SetSaltAndHash(user, newPassword);

            ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Edit,
                ActivityLogDetails.UserActionDetails.ResetPassword, true);
            return ValidationResult.Success;
        }

        public ValidatedResult<UserContactMethod> AddContactMethod(long userId, UserContactMethod contactMethod,
            bool shouldBeVerifyable)
        {
            var newContactMethodValidatedResult = BuildContactMethodForAddition(contactMethod, userId);
            if (!newContactMethodValidatedResult.IsValid)
                return newContactMethodValidatedResult;

            var newContactMethod = newContactMethodValidatedResult.Result;
            if (shouldBeVerifyable && !newContactMethod.IsVerifiable)
            {
                return
                    ValidatedResult<UserContactMethod>.Failure(
                        AuthenticationValidationErrors.ContactMethodShouldBeVerifyable);
            }

            var duplicateCount = DbManager.Db.UserContactMethods.Count(cm => !cm.IsDeleted &&
                                                                             cm.ContactMethodType ==
                                                                             newContactMethod.ContactMethodType &&
                                                                             cm.ContactMethodText ==
                                                                             newContactMethod.ContactMethodText &&
                                                                             cm.UserID == userId);

            if (duplicateCount > 0)
                return
                    ValidatedResult<UserContactMethod>.Failure(
                        AuthenticationValidationErrors.DuplicateContactMethodForUser);

            DbManager.Db.UserContactMethodsDbSet.Add(newContactMethod);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.User, userId, ActivityAction.CreateDetail,
                ActivityLogDetails.UserActionDetails.CreateContactMethod,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: newContactMethod.ID);

            return ValidatedResult<UserContactMethod>.Success(newContactMethod);
        }

        public void DeleteContactMethod(long contactMethodId)
        {
            var contactMethod =
                DbManager.Db.UserContactMethodsDbSet.Include(cm => cm.User)
                    .SingleOrDefault(cm => cm.ID == contactMethodId);

            if (contactMethod?.User == null)
                throw new ArgumentException("Unknown Contact Method ID or User");

            contactMethod.IsDeleted = true;
            ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.DeleteDetail,
                ActivityLogDetails.UserActionDetails.DeleteContactMethod,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID);

            if (contactMethod.User.IsVerified)
            {
                // Need to re-calculate the verification, or possibly clear it if no other verified contact methods present.
                DbManager.SaveDefaultDbChanges();
                RecalculateUserVerification(contactMethod.User);
            }
        }

        public UserContactMethodVerificationInfo LoadContactMethodVerificationInfo(long contactMethodId)
        {
            var contactMethod = DbManager.Db.UserContactMethods.SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null)
                return null;

            var lastLog =
                DbManager.Db.UserContactMethodVerifications.Where(l => l.TargetContactMethodID == contactMethodId)
                    .OrderByDescending(l => l.StartTime)
                    .FirstOrDefault();
            var result = new UserContactMethodVerificationInfo
            {
                ContactMethod = contactMethod,
                LatestVerification = lastLog,
                ValidationForVerificationStart = ValidationResult.Success
            };

            if (!contactMethod.IsVerified && !result.HasOngoingVerification)
                result.ValidationForVerificationStart = ValidateContactMethodForVerification(contactMethod);

            return result;
        }

        public UserContactMethodVerificationInfo StartContactMethodVerification(long contactMethodId)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var contactMethod =
                DbManager.Db.UserContactMethods.Include(cm => cm.User).SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null)
                throw new ArgumentException("Unknown Contact Method ID");

            var lastLog =
                DbManager.Db.UserContactMethodVerifications.Where(l => l.TargetContactMethodID == contactMethodId)
                    .OrderByDescending(l => l.StartTime)
                    .FirstOrDefault();
            ValidationResult result = ValidateContactMethodForVerification(contactMethod);

            if (!result.IsValid)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID,
                    ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.StartContactMethodVerification,
                    false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.UserContactMethodVerification);

                return new UserContactMethodVerificationInfo
                {
                    ContactMethod = contactMethod,
                    LatestVerification = lastLog,
                    ValidationForVerificationStart = result
                };
            }

            string secret =
                CryptoRandomNumberUtil.GetNumericString(contactMethod.ContactMethodType == ContactMethodType.Phone
                    ? SecretLengthForSms
                    : SecretLengthForEmail);

            var log = new UserContactMethodVerification
            {
                StartTime = DateTime.Now,
                ExpirationTime =
                    DateTime.Now +
                    (contactMethod.ContactMethodType == ContactMethodType.Phone
                        ? ExpirationDelayForVerificationBySms
                        : ExpirationDelayForVerificationByEmail),
                CompletionTime = null,
                VerificationSecret = secret,
                SessionID = ServiceContext.CurrentSession.Record.ID,
                TargetUserID = contactMethod.UserID,
                TargetContactMethodID = contactMethod.ID
            };

            // First save DB changes in order to create entity IDs used in activity log and message queue
            DbManager.Db.UserContactMethodVerificationsDbSet.Add(log);
            DbManager.SaveDefaultDbChanges();

            if (contactMethod.ContactMethodType == ContactMethodType.Email)
                EmailNotificationService.SendVerification(contactMethod.User, contactMethod, log);
            else if (contactMethod.ContactMethodType == ContactMethodType.Phone)
                SmsNotificationService.SendVerification(contactMethod.User, contactMethod, log);

            ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.OtherDetail,
                ActivityLogDetails.UserActionDetails.StartContactMethodVerification, true,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                auditEntity: AuditEntityType.UserContactMethodVerification, auditEntityID: log.ID);

            return new UserContactMethodVerificationInfo
            {
                ContactMethod = contactMethod,
                LatestVerification = log,
                ValidationForVerificationStart = ValidationResult.Success
            };
        }

        public ValidationResult PerformContactMethodVerification(long contactMethodId, string secret)
        {
            var contactMethod =
                DbManager.Db.UserContactMethodsDbSet.Include(cm => cm.User)
                    .SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null)
                throw new ArgumentException("Unknown ContactMethodID");
            if (contactMethod.IsVerified)
                return ValidationResult.Success;
            if (contactMethod.IsDeleted)
                throw new ArgumentException("Contact Method is already deleted");

            var lastLog =
                DbManager.Db.UserContactMethodVerificationsDbSet.Where(l => l.TargetContactMethodID == contactMethodId)
                    .OrderByDescending(l => l.StartTime)
                    .FirstOrDefault();
            if (lastLog == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID,
                    ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.CompleteContactMethodVerification,
                    false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.UserContactMethodVerification);
                return ValidationResult.Failure(AuthenticationValidationErrors.VerificationNotStarted);
            }

            if (lastLog.ExpirationTime <= DateTime.Now)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID,
                    ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.CompleteContactMethodVerification,
                    false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.UserContactMethodVerification, auditEntityID: lastLog.ID);
                return ValidationResult.Failure(AuthenticationValidationErrors.VerificationDeadlineExpired);
            }

            ValidationResult result;

            if (!lastLog.VerificationSecret.Equals(secret.Trim()))
            {
                result = ValidationResult.Failure(AuthenticationValidationErrors.InvalidSecret);
                lastLog.ExpirationTime -= TimeSpan.FromTicks((lastLog.ExpirationTime - DateTime.Now).Ticks/2) +
                                          TimeSpan.FromMinutes(5);

                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID,
                    ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.CompleteContactMethodVerification,
                    false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.UserContactMethodVerification, auditEntityID: lastLog.ID);
            }
            else
            {
                result = ValidationResult.Success;
                lastLog.CompletionTime = DateTime.Now;
                contactMethod.IsVerified = true;

                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID,
                    ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.CompleteContactMethodVerification,
                    true,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.UserContactMethodVerification, auditEntityID: lastLog.ID);

                DbManager.SaveDefaultDbChanges();
                RecalculateUserVerification(contactMethod.User);
                FixSavedSearchesAfterContactMethodVerification(contactMethod);
                contactMethod.User.ModificationDate = DateTime.Now;

                if (contactMethod.User.IsVerified)
                {
                    // Possibly reward new user signup bonus
                    // First save all changes to DB for the billing queries to work well.

                    DbManager.SaveDefaultDbChanges();
                    PromotionalBonusService.CreateNewAccountSignupBonusIfApplicable(contactMethod.User.ID);
                }
            }

            return result;
        }

        public void PerformContactMethodVerificationAdministratively(long contactMethodId)
        {
            var contactMethod =
                DbManager.Db.UserContactMethodsDbSet.Include(cm => cm.User)
                    .SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null)
                throw new ArgumentException("Unknown ContactMethodID");
            if (contactMethod.IsVerified)
                throw new ArgumentException("Contact Method is already verified");

            // Not checking if the record is being deleted in the administrative verification

            contactMethod.IsVerified = true;
            ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.OtherDetail,
                ActivityLogDetails.UserActionDetails.CompleteContactMethodVerificationAdministratively,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID);

            DbManager.SaveDefaultDbChanges();
            RecalculateUserVerification(contactMethod.User);
            FixSavedSearchesAfterContactMethodVerification(contactMethod);
            contactMethod.User.ModificationDate = DateTime.Now;
        }

        public void CancelContactMethodVerification(long contactMethodId)
        {
            var lastLog =
                DbManager.Db.UserContactMethodVerificationsDbSet.Where(l => l.TargetContactMethodID == contactMethodId)
                    .OrderByDescending(l => l.StartTime)
                    .FirstOrDefault();
            if (lastLog != null)
                if (lastLog.ExpirationTime > DateTime.Now && !lastLog.CompletionTime.HasValue)
                {
                    lastLog.ExpirationTime = DateTime.Now;
                    ActivityLogService.ReportActivity(TargetEntityType.User, lastLog.TargetUserID,
                        ActivityAction.OtherDetail, ActivityLogDetails.UserActionDetails.CancelContactMethodVerification,
                        detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethodId,
                        auditEntity: AuditEntityType.UserContactMethodVerification, auditEntityID: lastLog.ID);
                }
        }

        public ValidationResult StartPasswordRecovery(string loginName, ContactMethodType contactMethodType,
            string target)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            ValidationResult result;

            switch (contactMethodType)
            {
                case ContactMethodType.Phone:
                    result = ValidateStartPasswordRecoveryBySmsAndCanonicalize(ref target);
                    break;

                case ContactMethodType.Email:
                    result = ValidateStartPasswordRecoveryByEmailAndCanonicalize(ref target);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported contact method");
            }

            if (!result.IsValid)
                return result;

            var user =
                DbManager.Db.UsersDbSet.Include(u => u.ContactMethods).SingleOrDefault(u => u.LoginName == loginName);

            // Checking invalid request conditions.
            // RE-120 - These conditions are considered as failures, but the requester should not be informed of these conditions.
            // Otherwise, an intruder can use them to gain information about user accounts. So, return "success" without doing anything.

            if (user == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.StartPasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.PasswordResetRequest);
                return ValidationResult.Success;
            }

            UserContactMethod contactMethod = null;
            if (user.ContactMethods != null)
                contactMethod =
                    user.ContactMethods.FirstOrDefault(
                        cm =>
                            cm.ContactMethodType == contactMethodType && cm.ContactMethodText.Equals(target) &&
                            !cm.IsDeleted);

            if (contactMethod == null)
            {
                if (contactMethodType == ContactMethodType.Email)
                    EmailNotificationService.NotifyNonRegisteredEmailUsedForPasswordRecovery(target, loginName);

                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.StartPasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.PasswordResetRequest);

                return ValidationResult.Success;
            }

            var verifiedContactMethod =
                user.ContactMethods.FirstOrDefault(
                    cm =>
                        cm.ContactMethodType == contactMethodType && cm.ContactMethodText.Equals(target) &&
                        !cm.IsDeleted && cm.IsVerified);
            if (verifiedContactMethod == null)
            {
                if (contactMethodType == ContactMethodType.Email)
                    EmailNotificationService.NotifyNonVerifiedEmailUsedForPasswordRecovery(target, loginName);

                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.StartPasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.PasswordResetRequest);

                return ValidationResult.Success;
            }

            if (!user.IsEnabled)
            {
                if (contactMethodType == ContactMethodType.Email)
                    EmailNotificationService.NotifyAccountHasBeenDisabled(user);

                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.StartPasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: verifiedContactMethod.ID,
                    auditEntity: AuditEntityType.PasswordResetRequest);

                return ValidationResult.Success;
            }

            var lastLog =
                DbManager.Db.PasswordResetRequests.Where(l => l.TargetUserID == user.ID)
                    .OrderByDescending(l => l.StartTime)
                    .Take(1)
                    .SingleOrDefault();
            if (lastLog != null && (lastLog.StartTime + MinimumTimeBetweenTwoPasswordRecoveryAttempts) > DateTime.Now)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.StartPasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: verifiedContactMethod.ID,
                    auditEntity: AuditEntityType.PasswordResetRequest, auditEntityID: lastLog.ID);

                return ValidationResult.Failure(AuthenticationValidationErrors.TooFrequentRequests);
            }

            var newLog = new PasswordResetRequest
            {
                StartTime = DateTime.Now,
                ExpirationTime = DateTime.Now + ExpirationDelayForPasswordReset,
                CompletionTime = null,
                PasswordResetToken = CryptoRandomNumberUtil.GetAlphaNumericString(8).ToLower(),
                SessionID = ServiceContext.CurrentSession.Record.ID,
                TargetUserID = user.ID,
                ContactMethod = verifiedContactMethod
            };

            if (contactMethodType == ContactMethodType.Email)
            {
                EmailNotificationService.SendPasswordReset(user, newLog.ContactMethod, newLog);
            }

            if (contactMethodType == ContactMethodType.Phone)
            {
                SmsNotificationService.SendPasswordReset(user, newLog);
                EmailNotificationService.NotifyPasswordRecoveryBySmsStarted(user, newLog.ContactMethod, newLog);
            }

            DbManager.Db.PasswordResetRequestsDbSet.Add(newLog);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                ActivityLogDetails.UserActionDetails.StartPasswordRecovery, true,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: verifiedContactMethod.ID,
                auditEntity: AuditEntityType.PasswordResetRequest, auditEntityID: newLog.ID);

            return ValidationResult.Success;
        }

        public ValidationResult PerformPasswordRecovery(string loginName, string passwordResetToken, string newPassword)
        {
            if (string.IsNullOrEmpty(loginName))
                throw new ArgumentNullException(nameof(loginName));

            if (string.IsNullOrEmpty(passwordResetToken))
                throw new ArgumentNullException(nameof(passwordResetToken));

            // RE-120: In any failure event, only inform the user of the failure, no details on why it is a failure.

            var user = DbManager.Db.UsersDbSet.SingleOrDefault(u => u.LoginName == loginName);
            if (user == null || !user.IsEnabled)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.CompletePasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.PasswordResetRequest);
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            var passwordErrors = new List<ValidationError>();
            ValidatePassword(user, newPassword, passwordErrors);
            if (passwordErrors.Count > 0)
                return new ValidationResult {Errors = passwordErrors};

            var lastLog =
                DbManager.Db.PasswordResetRequestsDbSet.Where(l => l.TargetUserID == user.ID && l.CompletionTime == null)
                    .OrderByDescending(l => l.StartTime)
                    .Take(1)
                    .SingleOrDefault();
            if (lastLog == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.CompletePasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.PasswordResetRequest);
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            if (!lastLog.PasswordResetToken.Equals(passwordResetToken.Trim().ToLower()))
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.CompletePasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: lastLog.ContactMethodID,
                    auditEntity: AuditEntityType.PasswordResetRequest, auditEntityID: lastLog.ID);
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            if (lastLog.ExpirationTime <= DateTime.Now || lastLog.CompletionTime.HasValue)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.CompletePasswordRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: lastLog.ContactMethodID,
                    auditEntity: AuditEntityType.PasswordResetRequest, auditEntityID: lastLog.ID);
                return ValidationResult.Failure(AuthenticationValidationErrors.RequestHasExpired);
            }

            lastLog.CompletionTime = DateTime.Now;
            PasswordHashUtil.SetSaltAndHash(user, newPassword);
            user.FailedLoginAttempts = 0; // Unlock the account if it is locked

            EmailNotificationService.NotifyPasswordRecoveryComplete(user, lastLog);

            ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Other,
                ActivityLogDetails.UserActionDetails.CompletePasswordRecovery, true,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: lastLog.ContactMethodID,
                auditEntity: AuditEntityType.PasswordResetRequest, auditEntityID: lastLog.ID);
            return ValidationResult.Success;
        }

        public ValidationResult PerformLoginNameRecovery(ContactMethodType contactMethodType, string target)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            switch (contactMethodType)
            {
                case ContactMethodType.Phone:
                    return PerformLoginNameRecoveryBySms(target);

                case ContactMethodType.Email:
                    return PerformLoginNameRecoveryByEmail(target);
            }

            throw new ArgumentException("Unsupported verification method: " + contactMethodType);
        }

        public UniqueVisitor LookupVisitor(string uniqueIdentifier)
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
                return null;

            uniqueIdentifier = uniqueIdentifier.ToLower();

            var visitor = DbManager.Db.UniqueVisitorsDbSet.SingleOrDefault(v => v.UniqueIdentifier == uniqueIdentifier);

            if (visitor == null)
                return null;

            visitor.LastVisitDate = DateTime.Now;

            return UniqueVisitor.Copy(visitor);
        }

        public UniqueVisitor CreateVisitor()
        {
            var visitor = new UniqueVisitor
            {
                CreationDate = DateTime.Now,
                LastVisitDate = DateTime.Now,
                UniqueIdentifier = Base32Url.ToBase32String(Guid.NewGuid().ToByteArray()).ToLower()
            };

            var freshDb = DbManager.GetFreshDb();
            freshDb.UniqueVisitorsDbSet.Add(visitor);
            DbManager.SaveDbChanges(freshDb);

            return UniqueVisitor.Copy(visitor);
        }

        public SessionInfo StartSession(HttpSession initialSessionData)
        {
            var record = new HttpSession
            {
                Type = initialSessionData.Type,
                HttpSessionID = initialSessionData.HttpSessionID,
                PrevHttpSessionID = initialSessionData.PrevHttpSessionID,
                UserAgent = initialSessionData.UserAgent,
                StartupUri = initialSessionData.StartupUri,
                ReferrerUri = initialSessionData.ReferrerUri,
                ClientAddress = initialSessionData.ClientAddress,
                UniqueVisitorID = initialSessionData.UniqueVisitorID,
                UserID = initialSessionData.UserID,
                Start = DateTime.Now
            };

            DbManager.Db.HttpSessionsDbSet.Add(record);
            DbManager.SaveDefaultDbChanges();

            var recordID = record.ID;
            var noTrackingRecord = DbManager.Db.HttpSessions.Single(r => r.ID == recordID);

            return new SessionInfo {Record = noTrackingRecord};
        }

        public void AcknowledgeCurrentSessionAsInteractive()
        {
            if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.Record == null ||
                ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            if (ServiceContext.CurrentSession.Record.GotInteractiveAck)
                return;

            var sessionRecordId = ServiceContext.CurrentSession.Record.ID;
            var dbRecord = DbManager.Db.HttpSessionsDbSet.SingleOrDefault(s => s.ID == sessionRecordId);

            if (dbRecord == null)
                return;

            ServiceContext.CurrentSession.Record.GotInteractiveAck = true;
            dbRecord.GotInteractiveAck = true;
        }

        public void MarkSessionAsAuthenticated(long sessionId, long userId)
        {
            var freshDb = DbManager.GetFreshDb();
            var session = freshDb.HttpSessionsDbSet.SingleOrDefault(s => s.ID == sessionId);
            if (session == null)
                throw new InvalidOperationException("Session ID " + sessionId + " not found on the database.");

            if (session.UserID.HasValue)
            {
                if (session.UserID.Value == sessionId)
                    return;

                throw new InvalidOperationException("The session ID " + sessionId +
                                                    " was previously authenticated for user ID " + session.UserID.Value +
                                                    ", but it is being marked to be authenticated as another user, with ID " +
                                                    userId);
            }

            session.UserID = userId;
        }

        public void EndSession(SessionInfo sessionInfo, SessionEndReason reason)
        {
            if (sessionInfo?.Record == null)
                return;

            var dbRecord = DbManager.Db.HttpSessionsDbSet.Find(sessionInfo.Record.ID);

            if (dbRecord.End.HasValue)
            {
                // The session is already ended on the database previously, so ignore the end request.
                return;
            }

            sessionInfo.Record.End = DateTime.Now;
            sessionInfo.Record.EndReason = reason;

            dbRecord.End = sessionInfo.Record.End;
            dbRecord.EndReason = sessionInfo.Record.EndReason;
        }

        public void CleanupSessionsOnApplicationStartup()
        {
            var sixHoursAgo = DateTime.Now.AddHours(-6);

            var sessionAuditRecords = DbManager.Db.HttpSessionsDbSet.Where(r =>
                (r.Type == SessionType.Web && r.End == null) ||
                (r.Type == SessionType.Api && r.End == null && r.Start < sixHoursAgo))
                .ToList();
            foreach (var sessionAuditRecord in sessionAuditRecords)
            {
                sessionAuditRecord.End = DateTime.Now;
                sessionAuditRecord.EndReason = sessionAuditRecord.Type == SessionType.Web
                    ? SessionEndReason.ServerCrash
                    : SessionEndReason.Timeout;
            }
        }

        public void CleanupSessionsOnApplicationShutdown()
        {
            using (DbManager.StartThreadBoundScope())
            {
                var sessionAuditRecords = DbManager.Db.HttpSessionsDbSet.Where(r => r.End == null).ToList();
                foreach (var sessionAuditRecord in sessionAuditRecords)
                {
                    sessionAuditRecord.End = DateTime.Now;
                    sessionAuditRecord.EndReason = SessionEndReason.ServerShutdown;
                }
            }
        }

        #endregion

        #region Private helper methods

        private AuthenticationResult AuthenticatePassword(PasswordAuthenticationRequest request)
        {
            User user = DbManager.Db.UsersDbSet.SingleOrDefault(u => u.LoginName == request.LoginName);

            if (user == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Authenticate,
                    ActivityLogDetails.UserActionDetails.Login, false);
                return new FailedAuthenticationResult {ErrorKey = AuthenticationValidationErrors.UserNotFound};
            }

            if (user.FailedLoginAttempts >= 0 && user.LastLoginAttempt.HasValue &&
                (DateTime.Now - user.LastLoginAttempt.Value) > LockoutDuration)
                user.FailedLoginAttempts = 0;
                    // So that if failed after lockout time, count failures from 1, not LockoutFailedLogins + 1

            if (user.FailedLoginAttempts >= LockoutFailedLogins)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Authenticate,
                    ActivityLogDetails.UserActionDetails.Login, false);
                return new FailedAuthenticationResult
                {
                    ErrorKey = AuthenticationValidationErrors.AccountIsLocked,
                    AccountIsLocked = true
                };
            }

            if (!PasswordHashUtil.VerifySaltAndHash(user, request.Password))
            {
                user.LastLoginAttempt = DateTime.Now;
                user.FailedLoginAttempts++;

                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Authenticate,
                    ActivityLogDetails.UserActionDetails.Login, false);
                return new FailedAuthenticationResult {ErrorKey = AuthenticationValidationErrors.PasswordDoesNotMatch};
            }

            user.LastLogin = user.LastLoginAttempt = DateTime.Now;
            user.FailedLoginAttempts = 0;

            if (!user.IsEnabled)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Authenticate,
                    ActivityLogDetails.UserActionDetails.Login, false);
                return new FailedAuthenticationResult
                {
                    ErrorKey = AuthenticationValidationErrors.AccountIsDisabled,
                    AccountIsInactive = true
                };
            }

            ActivityLogService.ReportActivity(TargetEntityType.User, user.ID, ActivityAction.Authenticate,
                ActivityLogDetails.UserActionDetails.Login, true);
            return new SuccessfulAuthenticationResult {Principal = new CorePrincipal(user)};
        }

        private void ValidateUserForCreation(User user, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(user.LoginName))
                errors.Add(new ValidationError("LoginName", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (user.LoginName.Length > 50 || user.LoginName.Length < 4)
                    errors.Add(new ValidationError("LoginName",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));
                else
                {
                    if (CheckLoginNameExists(user.LoginName))
                        errors.Add(new ValidationError(AuthenticationValidationErrors.LoginNameIsAlreadyTaken));
                }
            }

            if (string.IsNullOrWhiteSpace(user.DisplayName))
                errors.Add(new ValidationError("DisplayName", GeneralValidationErrors.ValueNotSpecified));
            if (string.IsNullOrWhiteSpace(user.FullName))
                errors.Add(new ValidationError("FullName", GeneralValidationErrors.ValueNotSpecified));
            if (user.Type == UserType.IndependentAgencyMember && user.AgencyID == null)
                errors.Add(new ValidationError("IndependentAgencyMember", GeneralValidationErrors.ValueNotSpecified));

        }

        private void ValidatePassword(User user, string password, List<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(password))
                errors.Add(new ValidationError("Password", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (password.Length < 4)
                    errors.Add(new ValidationError("Password", GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));

                ValidatePasswordInclusion(password, user.LoginName,
                    AuthenticationValidationErrors.PasswordCantContainLoginName, errors);

                if (errors.Count > 0) // RE-159
                    return;

                ValidatePasswordInclusion(password, user.DisplayName,
                    AuthenticationValidationErrors.PasswordCantContainDisplayName, errors);

                if (errors.Count > 0) // RE-159
                    return;

                ValidatePasswordInclusion(password, user.FullName,
                    AuthenticationValidationErrors.PasswordCantContainFullName, errors);
            }
        }

        private void ValidatePasswordInclusion(string password, string propertyValue, string errorKey,
            List<ValidationError> errors)
        {
            if (!string.IsNullOrWhiteSpace(propertyValue))
            {
                if (password.ToLowerInvariant().Contains(propertyValue.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
                else if (propertyValue.ToLowerInvariant().Contains(password.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
            }
        }

        private User BuildUserForCreation(User original)
        {
            if (original == null)
                return null;

            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var user = new User();

            user.CreationDate = user.ModificationDate = DateTime.Now;
            user.CreatorSessionID = ServiceContext.CurrentSession.Record.ID;

            user.IsOperator = false;
            user.LoginName = original.LoginName?.Trim().ToLowerInvariant();
            user.IsEnabled = true;

            user.DisplayName = original.DisplayName?.Trim();
            user.FullName = original.FullName?.Trim();

            user.ShowInUsersList = original.ShowInUsersList;
            user.Activity = original.Activity;
            user.About = original.About;
            user.Services = original.Services;
            user.WorkBackground = original.WorkBackground;
            user.Address = original.Address;
            user.WebSiteUrl = original.WebSiteUrl;

            user.IsVerified = false;
            user.ReputationScore = 0;

            user.Type = original.Type;
            user.AgencyID = original.AgencyID;

            user.Approved = true;

            user.ContactMethods = new Collection<UserContactMethod>();
            foreach (var contactMethod in original.ContactMethods ?? Enumerable.Empty<UserContactMethod>())
            {
                var newContactMethod = BuildContactMethodForAddition(contactMethod, 0);
                if (!newContactMethod.IsValid)
                    throw new ArgumentException("Invalid contact method: " + contactMethod.ContactMethodText);

                user.ContactMethods.Add(newContactMethod.Result);
            }

            return user;
        }

        private ValidatedResult<UserContactMethod> BuildContactMethodForAddition(UserContactMethod original, long userId)
        {
            var newContactMethod = new UserContactMethod
            {
                ContactMethodType = original.ContactMethodType,
                ContactMethodText = original.ContactMethodText,
                Visibility = original.Visibility,
                IsActive = true,
                IsDeleted = false,
                IsVerified = false,
                UserID = userId
            };

            if (original.ContactMethodType == ContactMethodType.Phone)
            {
                var parsedPhoneNumber = LocalPhoneNumberUtils.ParseAndValidate(original.ContactMethodText);

                string formattedNumber = LocalPhoneNumberUtils.Format(parsedPhoneNumber);
                if (string.IsNullOrEmpty(formattedNumber))
                    return ValidatedResult<UserContactMethod>.Failure(AuthenticationValidationErrors.InvalidPhoneNumber);

                newContactMethod.ContactMethodText = formattedNumber;
                newContactMethod.IsVerifiable = ValidateContactMethodTextForVerification(newContactMethod).IsValid;
            }
            else if (original.ContactMethodType == ContactMethodType.Email)
            {
                if (!EmailUtils.IsValidEmail(original.ContactMethodText))
                    return ValidatedResult<UserContactMethod>.Failure(AuthenticationValidationErrors.InvalidEmailAddress);

                newContactMethod.IsVerifiable = true;
            }

            return ValidatedResult<UserContactMethod>.Success(newContactMethod);
        }

        private ValidationResult ValidateStartPasswordRecoveryBySmsAndCanonicalize(ref string phoneNumber)
        {
            if (!LocalPhoneNumberUtils.IsPossibleNumber(phoneNumber))
                return ValidationResult.Failure(AuthenticationValidationErrors.NotAPossiblePhoneNumber);

            var parsedNumber = LocalPhoneNumberUtils.ParseAndValidate(phoneNumber);
            if (parsedNumber == null)
                return ValidationResult.Failure(AuthenticationValidationErrors.InvalidPhoneNumber);

            if (parsedNumber.CountryCode != NationalLocaleCode)
                return ValidationResult.Failure(AuthenticationValidationErrors.OnlyNationalNumbersAllowed);

            if (LocalPhoneNumberUtils.GetNumberType(parsedNumber) != PhoneNumberType.MOBILE)
                return ValidationResult.Failure(AuthenticationValidationErrors.OnlyMobileNumbersAllowed);

            phoneNumber = LocalPhoneNumberUtils.Format(parsedNumber);
            return ValidationResult.Success;
        }

        private ValidationResult ValidateStartPasswordRecoveryByEmailAndCanonicalize(ref string emailAddress)
        {
            emailAddress = emailAddress.Trim().ToLower();
            if (!EmailUtils.IsValidEmail(emailAddress))
                return ValidationResult.Failure(AuthenticationValidationErrors.InvalidEmailAddress);

            return ValidationResult.Success;
        }

        private ValidationResult PerformLoginNameRecoveryBySms(string phoneNumber)
        {
            if (!LocalPhoneNumberUtils.IsPossibleNumber(phoneNumber))
                return ValidationResult.Failure(AuthenticationValidationErrors.NotAPossiblePhoneNumber);

            var parsedNumber = LocalPhoneNumberUtils.ParseAndValidate(phoneNumber);
            if (parsedNumber == null)
                return ValidationResult.Failure(AuthenticationValidationErrors.InvalidPhoneNumber);

            if (parsedNumber.CountryCode != NationalLocaleCode)
                return ValidationResult.Failure(AuthenticationValidationErrors.OnlyNationalNumbersAllowed);

            if (LocalPhoneNumberUtils.GetNumberType(parsedNumber) != PhoneNumberType.MOBILE)
                return ValidationResult.Failure(AuthenticationValidationErrors.OnlyMobileNumbersAllowed);

            var formattedNumber = LocalPhoneNumberUtils.Format(parsedNumber);

            var contactMethod =
                DbManager.Db.UserContactMethods.Include(cm => cm.User)
                    .FirstOrDefault(
                        cm =>
                            cm.ContactMethodType == ContactMethodType.Phone && cm.ContactMethodText == formattedNumber &&
                            cm.IsVerified && !cm.IsDeleted);

            // If the contact method is not found or there's an issue with the user account, we don't want the anonymous user to
            // get such information by using the login name recovery. So, just return success result without doing anything.

            if (contactMethod == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.LoginNameQuery);
                return ValidationResult.Success;
            }

            if (contactMethod.User == null || !contactMethod.User.IsEnabled)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.LoginNameQuery);
                return ValidationResult.Success;
            }

            // Check if the user hasn't requested the login name recently
            var lastLog =
                DbManager.Db.LoginNameQueries.Where(l => l.TargetUserID == contactMethod.UserID)
                    .OrderByDescending(l => l.QueryTime)
                    .FirstOrDefault();
            if (lastLog != null && (lastLog.QueryTime + MinimumTimeBetweenTwoLoginNameRecoveryAttempts) > DateTime.Now)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.LoginNameQuery, auditEntityID: lastLog.ID);
                return ValidationResult.Failure(AuthenticationValidationErrors.TooFrequentRequests);
            }

            SmsNotificationService.SendLoginNameQuery(contactMethod.User, contactMethod);
            var newLog = new LoginNameQuery
            {
                QueryTime = DateTime.Now,
                SessionID = ServiceContext.CurrentSession.Record.ID,
                ContactMethodID = contactMethod.ID,
                TargetUserID = contactMethod.UserID
            };

            DbManager.Db.LoginNameQueriesDbSet.Add(newLog);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, true,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                auditEntity: AuditEntityType.LoginNameQuery, auditEntityID: newLog.ID);
            return ValidationResult.Success;
        }

        private ValidationResult PerformLoginNameRecoveryByEmail(string emailAddress)
        {
            emailAddress = emailAddress.Trim().ToLower();
            if (!EmailUtils.IsValidEmail(emailAddress))
                return ValidationResult.Failure(AuthenticationValidationErrors.InvalidEmailAddress);

            var contactMethod =
                DbManager.Db.UserContactMethods.Include(cm => cm.User)
                    .FirstOrDefault(
                        cm =>
                            cm.ContactMethodType == ContactMethodType.Email && cm.ContactMethodText == emailAddress &&
                            cm.IsVerified && !cm.IsDeleted);

            // If the contact method is not found or there's an issue with the user account, we don't want the anonymous user to
            // get such information by using the login name recovery. So, just return success result without doing anything.

            if (contactMethod == null)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, null, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod,
                    auditEntity: AuditEntityType.LoginNameQuery);
                return ValidationResult.Success;
            }

            if (contactMethod.User == null || !contactMethod.User.IsEnabled)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.LoginNameQuery);
                return ValidationResult.Success;
            }

            // Check if the user hasn't requested the login name recently
            var lastLog =
                DbManager.Db.LoginNameQueries.Where(l => l.TargetUserID == contactMethod.UserID)
                    .OrderByDescending(l => l.QueryTime)
                    .FirstOrDefault();
            if (lastLog != null && (lastLog.QueryTime + MinimumTimeBetweenTwoLoginNameRecoveryAttempts) > DateTime.Now)
            {
                ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                    ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, false,
                    detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                    auditEntity: AuditEntityType.LoginNameQuery, auditEntityID: lastLog.ID);
                return ValidationResult.Failure(AuthenticationValidationErrors.TooFrequentRequests);
            }

            EmailNotificationService.SendLoginNameQuery(contactMethod.User, contactMethod);
            var newLog = new LoginNameQuery
            {
                QueryTime = DateTime.Now,
                SessionID = ServiceContext.CurrentSession.Record.ID,
                ContactMethodID = contactMethod.ID,
                TargetUserID = contactMethod.UserID
            };

            DbManager.Db.LoginNameQueriesDbSet.Add(newLog);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.User, contactMethod.UserID, ActivityAction.Other,
                ActivityLogDetails.UserActionDetails.AttemptLoginNameRecovery, true,
                detailEntity: DetailEntityType.UserContactMethod, detailEntityID: contactMethod.ID,
                auditEntity: AuditEntityType.LoginNameQuery, auditEntityID: newLog.ID);
            return ValidationResult.Success;
        }

        private VisibilityLevel FindCurrentUserVisibilityLevel(long forUserId)
        {
            if (ServiceContext.Principal == null || ServiceContext.Principal.CoreIdentity == null ||
                !ServiceContext.Principal.CoreIdentity.UserId.HasValue)
                return VisibilityLevel.Everybody;

            if (ServiceContext.Principal.CoreIdentity.UserId.Value == forUserId)
                return VisibilityLevel.System;

            if (ServiceContext.Principal.IsOperator)
                return VisibilityLevel.System;

            if (ServiceContext.Principal.IsVerified)
                return VisibilityLevel.ActivatedUsers;

            return VisibilityLevel.AllUsers;
        }

        private ValidationResult ValidateContactMethodForVerification(UserContactMethod contactMethod)
        {
            var result = ValidateContactMethodTextForVerification(contactMethod);

            if (result.IsValid)
                result = ValidateContactMethodDuplicationForVerification(contactMethod);

            if (result.IsValid)
                result = ValidateRequestFrequencyForContactMethodVerification(contactMethod);

            return result;
        }

        private ValidationResult ValidateContactMethodTextForVerification(UserContactMethod contactMethod)
        {
            if (contactMethod.IsVerified)
                return ValidationResult.Failure(AuthenticationValidationErrors.AlreadyVerified);

            if (contactMethod.ContactMethodType == ContactMethodType.Phone)
            {
                var phoneNumber = LocalPhoneNumberUtils.ParseAndValidate(contactMethod.ContactMethodText);
                if (phoneNumber == null)
                    return ValidationResult.Failure(AuthenticationValidationErrors.InvalidPhoneNumber);

                if (!LocalPhoneNumberUtils.IsNationalNumber(phoneNumber))
                    return ValidationResult.Failure(AuthenticationValidationErrors.OnlyNationalNumbersAllowed);

                if (!LocalPhoneNumberUtils.CanReceiveSms(phoneNumber))
                    return ValidationResult.Failure(AuthenticationValidationErrors.OnlyMobileNumbersAllowed);

                return ValidationResult.Success;
            }

            if (contactMethod.ContactMethodType == ContactMethodType.Email)
                return ValidationResult.Success;

            return ValidationResult.Failure(AuthenticationValidationErrors.UnsupportedContactMethodType);
        }

        private ValidationResult ValidateContactMethodDuplicationForVerification(UserContactMethod contactMethod)
        {
            var duplicateCount = DbManager.Db.UserContactMethods
                .Count(
                    cm =>
                        !cm.IsDeleted && cm.IsVerified && cm.ContactMethodType == contactMethod.ContactMethodType &&
                        cm.ContactMethodText == contactMethod.ContactMethodText);

            if (duplicateCount > 0)
                return ValidationResult.Failure(AuthenticationValidationErrors.DuplicateContactMethod);

            return ValidationResult.Success;
        }

        private ValidationResult ValidateRequestFrequencyForContactMethodVerification(UserContactMethod contactMethod)
        {
            // Find last log by matching contact method text, in order to prevent delete-and-add cycles repeatedly.

            // We are NOT filtering based on "IsDeleted" and "UserID" so that we prevent quickly-repeated contact method re-use
            // in the whole system (all users, including deleted ones)

            DateTime newestValidStartTime = DateTime.Now - MinimumTimeBetweenTwoContactMethodVerificationStarts;

            var lastLog = DbManager.Db.UserContactMethodVerifications
                .Where(l => l.StartTime > newestValidStartTime &&
                            l.TargetContactMethod.ContactMethodText == contactMethod.ContactMethodText &&
                            l.TargetContactMethod.ContactMethodType == contactMethod.ContactMethodType)
                .OrderByDescending(l => l.StartTime)
                .FirstOrDefault();

            if (lastLog == null)
                return ValidationResult.Success;

            return ValidationResult.Failure(AuthenticationValidationErrors.TooFrequentContactMethodVerificationStarts);
        }

        private void RecalculateUserVerification(User user)
        {
            var userId = user.ID;

            var existingVerifiedContacts =
                DbManager.Db.UserContactMethods.Where(cm => !cm.IsDeleted && cm.IsVerified && cm.UserID == userId)
                    .ToList();
            user.IsVerified = existingVerifiedContacts.Any(cm => cm.ContactMethodType == ContactMethodType.Phone) &&
                              existingVerifiedContacts.Any(cm => cm.ContactMethodType == ContactMethodType.Email);
        }

        private void FixSavedSearchesAfterContactMethodVerification(UserContactMethod contactMethod)
        {
            if (contactMethod.ContactMethodType == ContactMethodType.Email)
            {
                var savedPropertySearches = DbManager.Db.SavedPropertySearchesDbSet
                    .Where(sps => sps.UserID == contactMethod.UserID && !sps.DeleteTime.HasValue)
                    .Include(sps => sps.EmailNotificationTarget)
                    .ToList();

                foreach (var savedSearch in savedPropertySearches)
                {
                    if (savedSearch.EmailNotificationTarget == null || savedSearch.EmailNotificationTarget.IsDeleted ||
                        !savedSearch.EmailNotificationTarget.IsVerified)
                        savedSearch.EmailNotificationTargetID = contactMethod.ID;
                }
            }
            else if (contactMethod.ContactMethodType == ContactMethodType.Phone)
            {
                var savedPropertySearches = DbManager.Db.SavedPropertySearchesDbSet
                    .Where(sps => sps.UserID == contactMethod.UserID && !sps.DeleteTime.HasValue)
                    .Include(sps => sps.SmsNotificationTarget)
                    .ToList();

                foreach (var savedSearch in savedPropertySearches)
                {
                    if (savedSearch.SmsNotificationTarget == null || savedSearch.SmsNotificationTarget.IsDeleted ||
                        !savedSearch.SmsNotificationTarget.IsVerified)
                        savedSearch.SmsNotificationTargetID = contactMethod.ID;
                }
            }
        }

        #endregion
    }
}