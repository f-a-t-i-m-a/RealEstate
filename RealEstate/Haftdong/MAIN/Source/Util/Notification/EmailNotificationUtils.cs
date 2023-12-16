using System;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Resources;
using JahanJooy.RealEstateAgency.Util.Templates;
using JahanJooy.RealEstateAgency.Util.Templates.Email;
using log4net;

namespace JahanJooy.RealEstateAgency.Util.Notification
{
    [Contract]
    [Component]
    public class EmailNotificationUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EmailNotificationUtils));

        #region Component plugs

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public IEmailTransmitter EmailTransmitter { get; set; }

        #endregion

        public ValidationResult SendVerification(ApplicationUser user, ContactInfo contactInfo,
            ApplicationType applicationType)
        {
            if (contactInfo == null)
            {
                Log.ErrorFormat("The contact method is not Found");
                return ValidationResult.Failure("User.ContactMethod", GeneralValidationErrors.NotFound);
            }

            Log.DebugFormat("SendVerification for user {0}, contact method {1}", user.DisplayName,
                contactInfo.NormalizedValue);
            string secret = contactInfo.UserContactMethodVerification?.VerificationSecret;

            string subject = EmailResources.SendVerification_Subject;
            try
            {
                string body = "";
                IRazorTemplate contactMethodVerificationTemplate;
                var contactMethodVerificationModel = new ContactMethodVerificationModel
                {
                    User = user,
                    ContactMethodType = ContactMethodType.Email,
                    ContactMethodText = contactInfo.NormalizedValue,
                    VerificationSecret = secret
                };
                switch (applicationType)
                {
                    case ApplicationType.Haftdong:
                        contactMethodVerificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.HaftdongContactMethodVerification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                                .ApplyTemplate(contactMethodVerificationTemplate,
                                    contactMethodVerificationModel);
                        break;
                    case ApplicationType.Sheshdong:
                        contactMethodVerificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.SheshdongContactMethodVerification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                                .ApplyTemplate(contactMethodVerificationTemplate,
                                    contactMethodVerificationModel);
                        break;
                }

                var result = EmailTransmitter.Send(subject, body, new[] { contactInfo.NormalizedValue}, new string[0],
                    new string[0]);
                if (!result.IsValid)
                    Log.ErrorFormat("An error occured during sending Email to contact {0} of user with ID {1}", contactInfo.NormalizedValue, user.Id);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in sending verification email(s)", e);
                return ValidationResult.Failure("Verification.Mail.Send",
                    GeneralValidationErrors.UnexpectedValidationError);
            }

            return ValidationResult.Success;
        }

        public ValidationResult SendPasswordReset(ApplicationUser user, PasswordResetRequest passwordResetRequest,
            ApplicationType applicationType)
        {
            Log.DebugFormat("SendPasswordReset for user {0}, contact methdod {1}, request id {2}",
                user.Id, passwordResetRequest.ContactMethod, passwordResetRequest);

            if (passwordResetRequest.ContactMethod == null)
                throw new ArgumentException("ContactMethod is not filled for passwordResetRequest parameter.");
//            if (passwordResetRequest.ContactMethod.Emails.Count <= 0)
//                throw new ArgumentException("The password reset target method is not Email");
            try
            {
                string subject = EmailResources.PasswordReset_Subject;
                IRazorTemplate passwordResetTemplate;
                string body = "";

                var passwordResetModel = new PasswordResetModel
                {
                    User = user,
                    ApplicationUserContactMethod = passwordResetRequest.ContactMethod,
                    Request = passwordResetRequest
                };

                switch (applicationType)
                {
                    case ApplicationType.Haftdong:
                        passwordResetTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.HaftdongPasswordReset.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordResetTemplate,
                                passwordResetModel);
                        break;
                    case ApplicationType.Sheshdong:
                        passwordResetTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.SheshdongPasswordReset.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordResetTemplate,
                                passwordResetModel);
                        break;
                }

                var result = EmailTransmitter.Send(subject, body,
                    new[] {passwordResetRequest.ContactMethod.NormalizedValue},
                    new string[0], new string[0]);
                if (!result.IsValid)
                    return result;
            }
            catch (
                Exception e)
            {
                Log.Error("An exception has been occured in sending verification email(s)", e);
                return ValidationResult.Failure("Verification.Mail.Send",
                    GeneralValidationErrors.UnexpectedValidationError);
            }

            return ValidationResult.Success;
        }

        public ValidationResult NotifyPasswordRecoveryBySmsStarted(ApplicationUser targetUser,
            ContactInfo contactMethod,
            PasswordResetRequest passwordResetRequest, ApplicationType applicationType)
        {
            Log.DebugFormat("NotifyPasswordRecoveryBySmsStarted for user {0}, contact method {1}, request id {2}",
                targetUser.Id,
                contactMethod.NormalizedValue,
                passwordResetRequest.ID);

            var verifiedEmails =
                targetUser.Contact.Emails.Where(
                    cm => !cm.IsDeleted && cm.IsVerified)
                    .Select(cm => cm.NormalizedValue)
                    .ToArray();
            if (!verifiedEmails.Any())
                return ValidationResult.Failure("targetUser.ContactMethods", GeneralValidationErrors.NotFound);
            try
            {
                string subject = EmailResources.NotifyPasswordRecoveryBySmsStarted_Subject;
                IRazorTemplate passwordRecoveryBySmsStartNotificationTemplate;
                string body = "";

                var passwordRecoveryBySmsStartNotificationModel = new PasswordRecoveryBySmsStartNotificationModel
                {
                    User = targetUser,
                    ApplicationUserContactMethod = contactMethod,
                    Request = passwordResetRequest
                };
                switch (applicationType)
                {
                    case ApplicationType.Haftdong:
                        passwordRecoveryBySmsStartNotificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.HaftdongPasswordRecoveryBySmsStartNotification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordRecoveryBySmsStartNotificationTemplate,
                                passwordRecoveryBySmsStartNotificationModel);
                        break;
                    case ApplicationType.Sheshdong:
                        passwordRecoveryBySmsStartNotificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.SheshdongPasswordRecoveryBySmsStartNotification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordRecoveryBySmsStartNotificationTemplate,
                                passwordRecoveryBySmsStartNotificationModel);
                        break;
                }


                var result = EmailTransmitter.Send(subject, body, verifiedEmails, new string[0], new string[0]);
                if (!result.IsValid)
                    return result;
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in sending verification email(s)", e);
                return ValidationResult.Failure("Verification.Mail.Send",
                    GeneralValidationErrors.UnexpectedValidationError);
            }

            return ValidationResult.Success;
        }

        public ValidationResult NotifyPasswordRecoveryComplete(ApplicationUser user,
            PasswordResetRequest passwordResetRequest, ApplicationType applicationType)
        {
            Log.DebugFormat("NotifyPasswordRecoveryComplete for user {0}, request id {1}",
                user.Id,
                passwordResetRequest.ID);

            if (passwordResetRequest.ContactMethod == null)
                return ValidationResult.Success;


            try
            {
                string subject = EmailResources.NotifyPasswordRecoveryComplete_Subject;
                IRazorTemplate passwordRecoveryCompletionNotificationTemplate;
                string body = "";
                ContactMethodType contactMethodType;
                if (passwordResetRequest.ContactMethod is EmailInfo)
                    contactMethodType = ContactMethodType.Email;
                else if (passwordResetRequest.ContactMethod is PhoneInfo)
                    contactMethodType = ContactMethodType.Phone;
                else 
                    return ValidationResult.Failure("passwordResetRequest.ContactMethod", GeneralValidationErrors.NotValid);

                var passwordRecoveryCompletionNotificationModel = new PasswordRecoveryCompletionNotificationModel
                {
                    User = user,
                    ApplicationUserContactMethod = passwordResetRequest.ContactMethod,
                    Request = passwordResetRequest,
                    ContactMethodType = contactMethodType
                };

                switch (applicationType)
                {
                    case ApplicationType.Haftdong:
                        passwordRecoveryCompletionNotificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.HaftdongPasswordRecoveryCompletionNotification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordRecoveryCompletionNotificationTemplate,
                                passwordRecoveryCompletionNotificationModel);
                        break;
                    case ApplicationType.Sheshdong:
                        passwordRecoveryCompletionNotificationTemplate =
                            new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(),
                                "JahanJooy.RealEstateAgency.Util.Templates.Email.SheshdongPasswordRecoveryCompletionNotification.cshtml");
                        body = Composer.GetComponent<RazorTemplateRunner>()
                            .ApplyTemplate(passwordRecoveryCompletionNotificationTemplate,
                                passwordRecoveryCompletionNotificationModel);
                        break;
                }

                var result = EmailTransmitter.Send(subject, body,
                    new[] {passwordResetRequest.ContactMethod.NormalizedValue},
                    new string[0], new string[0]);
                if (!result.IsValid)
                    return result;
            }
            catch (
                Exception e)
            {
                Log.Error("An exception has been occured in sending verification email(s)", e);
                return ValidationResult.Failure("Verification.Mail.Send",
                    GeneralValidationErrors.UnexpectedValidationError);
            }

            return ValidationResult.Success;
        }
    }
}