using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Security;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.Notification;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using MongoDB.Bson;
using MongoDB.Driver;
using PhoneNumbers;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class UserUtil
    {
        private const int SecretLengthForSms = 6;
        private const int SecretLengthForEmail = 12;
        private static readonly TimeSpan ExpirationDelayForVerificationByEmail = TimeSpan.FromDays(1);
        private static readonly TimeSpan ExpirationDelayForVerificationBySms = TimeSpan.FromHours(6);
        private static readonly TimeSpan MinimumTimeBetweenTwoContactMethodVerificationStarts = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan MinimumTimeBetweenTwoPasswordRecoveryAttempts = TimeSpan.FromHours(1);
        private static readonly TimeSpan ExpirationDelayForPasswordReset = TimeSpan.FromHours(1);
        private static readonly ILog Log = LogManager.GetLogger(typeof(UserUtil));

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public EmailNotificationUtils EmailNotificationUtils { get; set; }

        [ComponentPlug]
        public SmsNotificationUtils SmsNotificationUtils { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalEmailUtil LocalEmailUtil { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; set; }

        public ValidatedResult<ApplicationUser> AddSecret(ApplicationUser user, ObjectId contactInfoId,
            ContactMethodType contactMethodType, ApplicationType applicationType)
        {
            string secret;
            if (ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Development)
            {
                secret = "1";
            }
            else
            {
                secret = CryptoRandomNumberUtil.GetNumericString(
                    contactMethodType == ContactMethodType.Phone ? SecretLengthForSms : SecretLengthForEmail);
            }

            var userContactMethodVerification = new UserContactMethodVerification
            {
                StartTime = DateTime.Now,
                ExpirationTime =
                    DateTime.Now +
                    (contactMethodType == ContactMethodType.Phone
                        ? ExpirationDelayForVerificationBySms
                        : ExpirationDelayForVerificationByEmail),
                CompletionTime = null,
                VerificationSecret = secret
            };

            var contactInfo = LocalContactMethodUtil.AddVerificationToContactInfo(user.Contact, contactInfoId,
                contactMethodType,
                userContactMethodVerification);

            var userFilter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(user.Id));
            var userUpdate = Builders<ApplicationUser>.Update
                .Set("Contact", user.Contact);

            var result = DbManager.ApplicationUser.UpdateOneAsync(userFilter, userUpdate).Result;

            if (result.MatchedCount != 1)
                return ValidatedResult<ApplicationUser>.Failure("User",
                    UserValidationError.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidatedResult<ApplicationUser>.Failure("User",
                    UserValidationError.NotModified);

            ValidationResult sendVerificationResult = new ValidationResult();
            if (contactMethodType == ContactMethodType.Email)
                sendVerificationResult = EmailNotificationUtils.SendVerification(user, contactInfo,
                    applicationType);
            else if (contactMethodType == ContactMethodType.Phone)
                sendVerificationResult = SmsNotificationUtils.SendVerification(user, contactInfo,
                    applicationType);

            if (!sendVerificationResult.IsValid)
            {
                return ValidatedResult<ApplicationUser>.Failure(sendVerificationResult.Errors);
            }

            return ValidatedResult<ApplicationUser>.Success(user);
        }

        public ValidatedResult<ApplicationUser> BuildContactMethodForAddition(string id,
            ContactMethodCollection applicationUserContactMethod)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var applicationUserUpdate = Builders<ApplicationUser>.Update
                .Set("Contact", applicationUserContactMethod);
            DbManager.ApplicationUser.FindOneAndUpdateAsync(filter, applicationUserUpdate);
            var user = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync().Result;
            if (user == null)
                return ValidatedResult<ApplicationUser>.Failure("User", UserValidationError.NotFound);

            return ValidatedResult<ApplicationUser>.Success(user);
        }

        public string GetUserName(ObjectId? userID)
        {
            if (userID == null || userID == ObjectId.Empty)
                return "ناشناس";

            var filter = Builders<ApplicationUser>.Filter.Eq("_id", userID);
            var user = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync();

            if (user != null)
            {
                var result = user.Result;
                return result?.DisplayName;
            }
            return "ناشناس";
        }

        public ApplicationUserDetails GetUserDetail(string id)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var user =
                DbManager.ApplicationUser.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            if (user == null)
                return null;

            if (!AuthorizeForView(user))
            {
                return new ApplicationUserDetails();
            }

            return Mapper.Map<ApplicationUserDetails>(user);
        }

        public ApplicationUser GetUser(string id)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var user =
                DbManager.ApplicationUser.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            if (user == null)
                return null;

            if (!AuthorizeForView(user))
            {
                return new ApplicationUser();
            }

            return user;
        }

        public ApplicationUserDetails GetMyProfile()
        {
            var id = OwinRequestScopeContext.Current.GetUserId();
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var user =
                DbManager.ApplicationUser.Find(filter)
                    .Project(p => Mapper.Map<ApplicationUserDetails>(p))
                    .SingleOrDefaultAsync()
                    .Result;

            return user;
        }

        public bool AuthorizeForView(ApplicationUser user)
        {
            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (user.CreatedByID != null &&
                user.CreatedByID.Equals(ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return true;
            }

            if (user.Id == OwinRequestScopeContext.Current.GetUserId())
            {
                return true;
            }

            return false;
        }

        public bool AuthorizeForEdit(ApplicationUser user)
        {
            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (user.CreatedByID != null &&
                user.CreatedByID.Equals(ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return true;
            }

            if (user.Id == OwinRequestScopeContext.Current.GetUserId())
            {
                return true;
            }

            return false;
        }

        public async Task<ValidatedResult<ContactInfo>> CompleteRegistrationAddNewContactMethodAsync(
            string userId, NewContactInfoInput input, ApplicationType applicationType)
        {
            var userFilter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(userId));
            var currentUser = await DbManager.ApplicationUser.Find(userFilter).SingleOrDefaultAsync();

            if (!AuthorizeForEdit(currentUser))
            {
                return ValidatedResult<ContactInfo>.Failure("User", GeneralValidationErrors.AccessDenied);
            }

            var contactInfo = LocalContactMethodUtil.MapAndAddContactMethod(currentUser.Contact, input);
            var contactResult = LocalContactMethodUtil.PrepareContactMethod(contactInfo, false, false, false);
            if (!contactResult.IsValid)
                return ValidatedResult<ContactInfo>.Failure(contactResult.Errors);

            var result = BuildContactMethodForAddition(userId, currentUser.Contact);
            if (!result.IsValid)
                return ValidatedResult<ContactInfo>.Failure(result.Errors);

            var user = AddSecret(currentUser, contactInfo.ID, input.Type, applicationType);

            if (!user.IsValid)
                return ValidatedResult<ContactInfo>.Failure(user.Errors);

            return ValidatedResult<ContactInfo>.Success(contactInfo);
        }

        public async Task<ValidatedResult<string>> CompleteRegistrationVerifyContactMethod(
            UserApplicationVerifyContactMethodInput input)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(input.UserID));
            var user = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync().Result;
            try
            {
                var contactInfo = LocalContactMethodUtil.GetContactInfo(user.Contact,
                    ObjectId.Parse(input.ContactMethodID));
                var result = contactInfo?.UserContactMethodVerification.ExpirationTime != null;
                if (!result)
                    return ValidatedResult<string>.Failure("User.ContactMethod",
                        GeneralValidationErrors.ValueNotSpecified);

                if (contactInfo.UserContactMethodVerification.ExpirationTime < DateTime.Now)
                {
                    return
                        ValidatedResult<string>.Failure(
                            "User.ContactMethod.UserContactMethodVerification.ExpirationTime",
                            UserValidationError.VerificationDeadlineExpired);
                }
                if (contactInfo.UserContactMethodVerification.VerificationSecret != input.VerificationSecret)
                {
                    return
                        ValidatedResult<string>.Failure(
                            "User.ContactMethod.UserContactMethodVerification.VerificationSecret",
                            UserValidationError.InvalidSecret);
                }

                contactInfo.UserContactMethodVerification.CompletionTime = DateTime.Now;
                contactInfo.IsVerified = true;
                LocalContactMethodUtil.UpdateContactInfo(user.Contact, contactInfo);

                var update = Builders<ApplicationUser>.Update
                    .Set("Contact", user.Contact);

                var updatedResult = DbManager.ApplicationUser.UpdateOneAsync(filter, update).Result;
                if (updatedResult.MatchedCount != 1)
                    return ValidatedResult<string>.Failure("User", UserValidationError.NotFound);

                if (updatedResult.MatchedCount == 1 && updatedResult.ModifiedCount != 1)
                    return ValidatedResult<string>.Failure("User", UserValidationError.NotModified);

                var users = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync().Result;
                if (LocalContactMethodUtil.HasVerifiedContactMethod(users.Contact))
                {
                    var updateUserVerification = Builders<ApplicationUser>.Update
                        .Set("IsVerified", true)
                        .Push("Roles", BuiltInRole.VerifiedUser.ToString());
                    await DbManager.ApplicationUser.UpdateOneAsync(filter, updateUserVerification);

                    var token = await UpdateToken();

                    return ValidatedResult<string>.Success(token);
                }

                return ValidatedResult<string>.Success("");
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured during Complete Registration Verify Contact Method", e);
                return ValidatedResult<string>.Failure("User", UserValidationError.UnexpectedError);
            }
        }

        public ValidationResult GetAnotherSecret(NewSecretForUserApplicationInput input, ApplicationType applicationType)
        {
            DateTime newestValidStartTime = DateTime.Now - MinimumTimeBetweenTwoContactMethodVerificationStarts;

            var userFilter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(input.UserID));
            var user = DbManager.ApplicationUser.Find(userFilter).SingleOrDefaultAsync().Result;
            try
            {
                var contactInfo = LocalContactMethodUtil.GetContactInfo(user.Contact,
                    ObjectId.Parse(input.ContactMethodID));

                if (contactInfo != null && !contactInfo.IsVerified &&
                    contactInfo.UserContactMethodVerification.CompletionTime == null)
                {
                    if (contactInfo.UserContactMethodVerification.StartTime > newestValidStartTime)
                    {
                        return ValidationResult.Failure("User.ContactMethod",
                            UserValidationError.IsTooFrequentContactMethodVerification);
                    }
                    if (contactInfo.UserContactMethodVerification.ExpirationTime > DateTime.Now)
                    {
                        if (contactInfo is EmailInfo)
                            EmailNotificationUtils.SendVerification(user, contactInfo, applicationType);
                        if (contactInfo is PhoneInfo)
                            SmsNotificationUtils.SendVerification(user, contactInfo, applicationType);

                        UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                        {
                            ApplicationType = applicationType,
                            TargetType = EntityType.ApplicationUser,
                            TargetID = ObjectId.Parse(input.UserID),
                            ActivitySubType = "SendVerification"
                        });
                        return ValidationResult.Success;
                    }
                    var result = AddSecret(user, contactInfo.ID,
                        LocalContactMethodUtil.GetTypeOfContactInfo(contactInfo),
                        applicationType);

                    if (result.IsValid)
                    {
                        UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                        {
                            ApplicationType = applicationType,
                            TargetType = EntityType.ApplicationUser,
                            TargetID = ObjectId.Parse(input.UserID),
                            ActivitySubType = "SendVerification"
                        });

                        return ValidationResult.Success;
                    }
                    return new ValidationResult
                    {
                        Errors = result.Errors
                    };
                }
                return ValidationResult.Failure("User.ContactMethod", GeneralValidationErrors.ValueNotSpecified);
            }
            catch (Exception e)
            {
                Log.Error("an exception has been occured during Get New Secret", e);
                return ValidationResult.Failure("User", UserValidationError.UnexpectedError);
            }
        }

        public ValidatedResult<ApplicationUser> DeleteContactMethod(string id, ApplicationType applicationType)
        {
            try
            {
                var filter = Builders<ApplicationUser>.Filter.Eq("Contact.Phones.ID", ObjectId.Parse(id))
                             | Builders<ApplicationUser>.Filter.Eq("Contact.Emails.ID", ObjectId.Parse(id))
                             | Builders<ApplicationUser>.Filter.Eq("Contact.Addresses.ID", ObjectId.Parse(id));
                var user = DbManager.ApplicationUser.Find(filter).SingleAsync().Result;
                if (user == null)
                    return ValidatedResult<ApplicationUser>.Failure("User", UserValidationError.NotFound);
                if (!AuthorizeForEdit(user))
                {
                    return ValidatedResult<ApplicationUser>.Failure("User", GeneralValidationErrors.AccessDenied);
                }

                user.Contact.Phones?.RemoveAll(p => p.ID == ObjectId.Parse(id));
                user.Contact.Emails?.RemoveAll(p => p.ID == ObjectId.Parse(id));
                user.Contact.Addresses?.RemoveAll(p => p.ID == ObjectId.Parse(id));
                user.IsVerified = LocalContactMethodUtil.HasVerifiedContactMethod(user.Contact);
                var update = Builders<ApplicationUser>.Update
                    .Set("Contact", user.Contact)
                    .Set("IsVerified", user.IsVerified)
                    .Set("LastIndexingTime", BsonNull.Value);

                var result = DbManager.ApplicationUser.UpdateOneAsync(filter, update);
                if (result.Result.MatchedCount != 1)
                    return ValidatedResult<ApplicationUser>.Failure("User", UserValidationError.NotFound);

                if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                    return ValidatedResult<ApplicationUser>.Failure("User", UserValidationError.NotModified);

                return ValidatedResult<ApplicationUser>.Success(user);
            }
            catch (Exception e)
            {
                Log.Error("an exception has been occured during Get New Secret", e);
                return ValidatedResult<ApplicationUser>.Failure("User", UserValidationError.UnexpectedError);
            }
        }

        public ContactMethodCollection GetContactInfoOfCurrentUser()
        {
            var userId = OwinRequestScopeContext.Current.GetUserId();
            var existingUser = GetUser(userId);
            return existingUser?.Contact;
        }

        public ValidationResult StartPasswordRecovery(AccountForgotPasswordInput input,
            ContactMethodType contactMethodType, ApplicationType applicationType)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq("UserName", input.UserName);
            var user = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync().Result;

            // Checking invalid request conditions.
            // RE-120 - These conditions are considered as failures, but the requester should not be informed of these conditions.
            // Otherwise, an intruder can use them to gain information about user accounts. So, return "success" without doing anything.

            if (user == null)
            {
                Log.InfoFormat("User with userName {0} not fond", input.UserName);
                return ValidationResult.Failure("User", GeneralValidationErrors.NotFound);
            }

            ValidationResult result;
            ContactInfo verifiedContactMethod;
            switch (contactMethodType)
            {
                case ContactMethodType.Phone:

                    PhoneNumber normalizedInputNumber;
                    int countryCode;
                    string areaCode;
                    LocalPhoneNumberUtil.NormalizePhoneNumber(input.VerificationPhoneNumber, out normalizedInputNumber,
                        out countryCode, out areaCode);
                    var formattedNumber = LocalPhoneNumberUtil.Format(normalizedInputNumber);
                    if (string.IsNullOrEmpty(formattedNumber))
                    {
                        Log.InfoFormat("unvalid number {0} ", input.VerificationPhoneNumber);
                        return ValidationResult.Success;
                    }

                    verifiedContactMethod =
                        user.Contact.Phones.FirstOrDefault(cm => !cm.IsDeleted && cm.NormalizedValue == formattedNumber);
                    if (verifiedContactMethod == null)
                    {
                        Log.InfoFormat(
                            "User {0} does not have verified Contact Method,or Phone Number {1} is not match with Verified ContactMethod ",
                            user.DisplayName, input.VerificationPhoneNumber);
                        return ValidationResult.Success;
                    }
                    result = ValidateStartPasswordRecoveryBySmsAndCanonicalize(normalizedInputNumber, user);
                    break;

                case ContactMethodType.Email:

                    verifiedContactMethod =
                        user.Contact.Emails.FirstOrDefault(
                            cm => !cm.IsDeleted && cm.NormalizedValue == input.VerificationEmailAddress);
                    if (verifiedContactMethod == null)
                    {
                        Log.InfoFormat(
                            "User {0} does not have verified Contact Method,or Email Address {1} is not match with Verified ContactMethod ",
                            user.DisplayName, input.VerificationEmailAddress);
                        return ValidationResult.Success;
                    }
                    result = ValidateStartPasswordRecoveryByEmailAndCanonicalize(verifiedContactMethod.NormalizedValue);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported contact method");
            }


            if (!result.IsValid)
                return result;

            var applicationUserRefrence = Mapper.Map<ApplicationUserReference>(user);

            var filterPasswordResetRequests1 = Builders<PasswordResetRequest>.Filter.Eq("User.Id", user.Id);
            var sort = Builders<PasswordResetRequest>.Sort.Descending("StartTime");
            var lastLog = DbManager.PasswordResetRequest.Find(filterPasswordResetRequests1).Sort(sort).FirstOrDefault();

            if (lastLog != null && (lastLog.StartTime + MinimumTimeBetweenTwoPasswordRecoveryAttempts) > DateTime.Now)
            {
                return ValidationResult.Failure(AuthenticationValidationErrors.TooFrequentRequests);
            }
            var newLog = new PasswordResetRequest
            {
                StartTime = DateTime.Now,
                ExpirationTime = DateTime.Now + ExpirationDelayForPasswordReset,
                CompletionTime = null,
                PasswordResetToken = CryptoRandomNumberUtil.GetAlphaNumericString(8).ToLower(),
                User = applicationUserRefrence,
                ContactMethod = verifiedContactMethod
            };

            if (contactMethodType == ContactMethodType.Email)
            {
                EmailNotificationUtils.SendPasswordReset(user, newLog, applicationType);
            }

            if (contactMethodType == ContactMethodType.Phone)
            {
                SmsNotificationUtils.SendPasswordReset(user, newLog, applicationType);
                EmailNotificationUtils.NotifyPasswordRecoveryBySmsStarted(user, newLog.ContactMethod, newLog,
                    applicationType);
            }
            DbManager.PasswordResetRequest.InsertOne(newLog);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = applicationType,
                TargetID = ObjectId.Parse(user.Id),
                TargetType = EntityType.ApplicationUser,
                ActivityType = UserActivityType.Other,
                Comment = "PasswordRecovery"
            });
            return ValidationResult.Success;
        }

        #region Private helper methods

        private ValidationResult ValidateStartPasswordRecoveryBySmsAndCanonicalize(PhoneNumber phoneNumber,
            ApplicationUser user)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            if (phoneNumberUtil.GetNumberType(phoneNumber) != PhoneNumberType.MOBILE)
            {
                Log.InfoFormat("User {0} has verified Phone Number {1} , that could not Receive Sms  ", user.DisplayName,
                    phoneNumber.NationalNumber);
                return ValidationResult.Failure(AuthenticationValidationErrors.OnlyMobileNumbersAllowed);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateStartPasswordRecoveryByEmailAndCanonicalize(string emailAddress)
        {
            emailAddress = emailAddress.Trim().ToLower();
            if (!LocalEmailUtil.IsValidEmail(emailAddress))
                return ValidationResult.Failure(AuthenticationValidationErrors.InvalidEmailAddress);
            return ValidationResult.Success;
        }

        private async Task<string> UpdateToken()
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(store);

            ApplicationUser user = userManager.FindByIdAsync(currentUserId).Result;

            if (user == null)
            {
                return "";
            }

            if (!user.IsEnabled)
            {
                return "";
            }

            try
            {
                ClaimsIdentity oAuthIdentity =
                    await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                var token = OAuthOptions.AccessTokenFormat.Protect(ticket);
                return token;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userName", userName}
            };
            return new AuthenticationProperties(data);
        }

        

        #endregion
    }
}