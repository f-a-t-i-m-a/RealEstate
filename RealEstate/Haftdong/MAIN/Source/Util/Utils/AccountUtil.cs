using System;
using System.Collections.Generic;
using System.Linq;
using AspNet.Identity.MongoDB;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Enums.Elastic;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.Notification;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using log4net;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class AccountUtil
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(AccountUtil));

        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public EmailNotificationUtils EmailNotificationUtils { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        #endregion

        #region Action methods

        public ValidationResult Signup(SignupInput input, ApplicationUser user, ApplicationType applicationType)
        {
            var preprationResult = PrepareUserForSignup(user);
            if (!preprationResult.IsValid)
                return preprationResult;

            var errors = ValidateUserForSignup(user);
            errors.AddRange(ValidatePassword(user, input.Password));

            if (errors.Count > 0)
                return new ValidationResult
                {
                    Errors = errors
                };

            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            var userManager = new UserManager<ApplicationUser>(store);

            var result = userManager.CreateAsync(user).Result;
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(user.Id), applicationType: applicationType);

            if (result.Errors.Any())
                errors.AddRange(
                    result.Errors.Select(
                        error =>
                            new ValidationError("User", UserValidationError.UnexpectedError, new[] { error })));
            if (errors.Any())
                return new ValidationResult
                {
                    Errors = errors
                };

            user.CreatedByID = ObjectId.Parse(user.Id);
            user.ModificationTime = DateTime.Now;
            var editResult = userManager.UpdateAsync(user);
            if (editResult.Result.Errors.Any())
                errors.AddRange(
                    result.Errors.Select(
                        error =>
                            new ValidationError("User.CreatedByID", UserValidationError.UnexpectedError,
                                new[] { error })));

            var passwordResult = userManager.AddPasswordAsync(user.Id, input.Password);
            if (passwordResult.Result.Errors.Any())
                errors.AddRange(
                    result.Errors.Select(
                        error =>
                            new ValidationError("User.Password", UserValidationError.UnexpectedError,
                                new[] { error })));
            if (errors.Any())
                return new ValidationResult
                {
                    Errors = errors
                };

            //Verification Proccess
            var addSecretErrors = new List<ValidatedResult<ApplicationUser>>();

            user.Contact?.Phones?.ForEach(p =>
            {
                if(p.IsVerifiable && p.CanReceiveSms)
                    addSecretErrors.Add(UserUtil.AddSecret(user, p.ID, LocalContactMethodUtil.GetTypeOfContactInfo(p), applicationType));
            });

            user.Contact?.Emails?.ForEach(e =>
            {
                if (e.IsVerifiable)
                    addSecretErrors.Add(UserUtil.AddSecret(user, e.ID, LocalContactMethodUtil.GetTypeOfContactInfo(e), applicationType));
            });

            if (addSecretErrors.Any(s => !s.IsValid))
            {
                var allErrors = addSecretErrors.FindAll(s => !s.IsValid).ToList();
                var returnResult = new ValidationResult
                {
                    Errors = new List<ValidationError>()
                };

                allErrors.ForEach(e =>
                {
                    returnResult.Errors.AddRange(e.Errors);
                });
                return returnResult;
            }

            return ValidationResult.Success;
        }

        public ValidationResult PerformPasswordRecovery(string userName, string passwordResetToken, string newPassword, ApplicationType applicationType)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(passwordResetToken))
                throw new ArgumentNullException(nameof(passwordResetToken));

            // RE-120: In any failure event, only inform the user of the failure, no details on why it is a failure.
            var userfilter = Builders<ApplicationUser>.Filter.Eq("UserName", userName);
            var user = DbManager.ApplicationUser.Find(userfilter).SingleOrDefaultAsync().Result;

            if (user == null )
            {
                Log.InfoFormat("User with userName {0} not fond", userName);
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(user.Id));

            var passwordErrors = new List<ValidationError>();
            ValidatePassword(user, newPassword);
            if (passwordErrors.Count > 0)
                return new ValidationResult { Errors = passwordErrors };

            var filterPasswordResetRequests = Builders<PasswordResetRequest>.Filter.Eq("User.Id", user.Id) & !Builders<PasswordResetRequest>.Filter.Eq("CompletionTime", "");
            var sort = Builders<PasswordResetRequest>.Sort.Descending("StartTime");
            var lastLog = DbManager.PasswordResetRequest.Find(filterPasswordResetRequests).Sort(sort).FirstOrDefault();

          
            if (lastLog == null)
            {
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            if (!lastLog.PasswordResetToken.Equals(passwordResetToken.Trim().ToLower()))
            {
                return ValidationResult.Failure(AuthenticationValidationErrors.PasswordResetTokensDontMatch);
            }

            if (lastLog.ExpirationTime <= DateTime.UtcNow || lastLog.CompletionTime.HasValue)
            {
                return ValidationResult.Failure(AuthenticationValidationErrors.RequestHasExpired);
            }

            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            var userManager = new UserManager<ApplicationUser>(store);
            userManager.RemovePassword(user.Id);

            var result = userManager.AddPasswordAsync(user.Id, newPassword);

            if (result.Result.Errors.Any())
            {
                return ValidationResult.Failure(GeneralValidationErrors.UnexpectedValidationError);
            }


            var updateUser = Builders<ApplicationUser>.Update
               .Set("FailedLoginAttempts", 0)// Unlock the account if it is locked
               .Set("ModificationTime", DateTime.Now);

            var updateUserResult = DbManager.ApplicationUser.UpdateOneAsync(userfilter, updateUser).Result;

            if (updateUserResult.MatchedCount != 1)
                return ValidationResult.Failure("User", GeneralValidationErrors.NotFound);

            if (updateUserResult.MatchedCount == 1 && updateUserResult.ModifiedCount != 1)
                return ValidationResult.Failure("User", GeneralValidationErrors.NotModified);

            var updatePasswordResetRequest = Builders<PasswordResetRequest>.Update
                .Set("CompletionTime", DateTime.Now);

            var updatePasswordResetRequestResult = DbManager.PasswordResetRequest.UpdateOneAsync(filterPasswordResetRequests, updatePasswordResetRequest).Result;

            if (updatePasswordResetRequestResult.MatchedCount != 1)
                return ValidationResult.Failure("PasswordResetRequest", GeneralValidationErrors.NotFound);

            if (updatePasswordResetRequestResult.MatchedCount == 1 && updatePasswordResetRequestResult.ModifiedCount != 1)
                return ValidationResult.Failure("PasswordResetRequest", GeneralValidationErrors.NotModified);

            EmailNotificationUtils.NotifyPasswordRecoveryComplete(user, lastLog, applicationType);
            return ValidationResult.Success;
        }

        #endregion

        #region Private helper methods 

        private ValidationResult PrepareUserForSignup(ApplicationUser user)
        {
            user.LastIndexingTime = null;
            user.CreationTime = DateTime.Now;
            user.IsEnabled = true;
            user.IsOperator = false;
            user.IsVerified = false;
            user.Type = UserType.NormalUser;
            user.Roles.Add(BuiltInRole.RealEstateAgent.ToString());
            user.LicenseType = LicenseType.Trial;
            user.LicenseActivationTime = DateTime.Now.AddDays(14);

            var contactResult = LocalContactMethodUtil.PrepareContactMethods(user.Contact, false, false, true);
            if (!contactResult.IsValid)
                return contactResult;

            return ValidationResult.Success;
        }

        private List<ValidationError> ValidateUserForSignup(ApplicationUser user)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(user.UserName))
                errors.Add(new ValidationError("User.UserName", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (user.UserName.Length > 50 || user.UserName.Length < 4)
                    errors.Add(new ValidationError("User.UserName",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));
                else
                {
                    if (CheckUserNameExists(user.UserName))
                        errors.Add(new ValidationError(AuthenticationValidationErrors.UserNameIsAlreadyTaken));
                }
            }

//            if (string.IsNullOrWhiteSpace(user.DisplayName))
//                errors.Add(new ValidationError("User.DisplayName", GeneralValidationErrors.ValueNotSpecified));
//            if (user.Contact?.Phones?.Count > 0 && (bool) !user.Contact?.Phones[0].CanReceiveSms)
//                errors.Add(new ValidationError("User.Contact.Phones", AuthenticationValidationErrors.ContactMethodShouldBeVerifyable));
//            if (user.Contact?.Phones?.Count <=0)
//                errors.Add(new ValidationError("User.Contact.Phones", GeneralValidationErrors.ValueNotSpecified));

            return errors;
        }

        private bool CheckUserNameExists(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            userName = userName.Trim().ToLowerInvariant();

            var response = ElasticManager.Client.Search<ApplicationUserIE>(u => u
                .Index(ElasticManager.Index)
                .Type(Types.UserType)
                .Query(q => q.Term(ur => ur.UserName, userName)
                )
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while checking username {0} is existed or not, debug information: {1}",
                    userName, response.DebugInformation);
            }

            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            return (ids.Count > 0);
        }

         private List<ValidationError> ValidatePassword(ApplicationUser user, string password)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(password))
                errors.Add(new ValidationError("User.Password", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (password.Length < 6)
                    errors.Add(new ValidationError("User.Password",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));

                errors.AddRange(ValidatePasswordInclusion(password, user.UserName,
                    AuthenticationValidationErrors.PasswordCantContainUserName));

                if (errors.Count > 0) // RE-159
                    return errors;

                errors.AddRange(ValidatePasswordInclusion(password, user.DisplayName,
                    AuthenticationValidationErrors.PasswordCantContainDisplayName));

                if (errors.Count > 0) // RE-159
                    return errors;

                errors.AddRange(ValidatePasswordInclusion(password, user.DisplayName,
                    AuthenticationValidationErrors.PasswordCantContainFullName));
            }

            return errors;
        }

        private List<ValidationError> ValidatePasswordInclusion(string password, string propertyValue, string errorKey)
        {
            var errors = new List<ValidationError>();
            if (!string.IsNullOrWhiteSpace(propertyValue))
            {
                if (password.ToLowerInvariant().Contains(propertyValue.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
                else if (propertyValue.ToLowerInvariant().Contains(password.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
            }
            return errors;
        }

        #endregion

    }
}