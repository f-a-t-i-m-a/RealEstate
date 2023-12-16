using System;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using log4net;

namespace JahanJooy.RealEstateAgency.Util.Notification
{
    [Contract]
    [Component]
    public class SmsNotificationUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SmsNotificationUtils));

        #region Component plugs

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ISmsTransmitter SmsTransmitter { get; set; }

        #endregion

        #region ISmsNotificationService implementation

        public ValidationResult SendVerification(ApplicationUser user,
            ContactInfo contactInfo, ApplicationType applicationType)
        {
            if (contactInfo?.UserContactMethodVerification == null)
            {
                Log.ErrorFormat("contactMethod is not found");
                return ValidationResult.Failure("User.ContactMethod", GeneralValidationErrors.NotFound);
            }

            string text = "";
            switch (applicationType)
            {
                case ApplicationType.Haftdong:
                    text = "رمز فعال سازی شماره شما در سایت هفت دنگ: " +
                           contactInfo.UserContactMethodVerification.VerificationSecret;
                    break;
                case ApplicationType.Sheshdong:
                    text = "رمز فعال سازی شماره شما در سایت شش دنگ: " +
                           contactInfo.UserContactMethodVerification.VerificationSecret;
                    break;
            }

            var result = SmsTransmitter.Send(contactInfo.NormalizedValue, text);
            if (!result.IsValid)
                Log.ErrorFormat("An error occured in sending SMS to contact {0} of User with ID {1}", contactInfo.NormalizedValue, user.Id);

            return ValidationResult.Success;
        }

        public ValidationResult SendPasswordReset(ApplicationUser user, PasswordResetRequest passwordResetRequest, ApplicationType applicationType)
        {
            if (passwordResetRequest.ContactMethod == null)
                throw new ArgumentException("ContactMethod is not filled for passwordResetRequest parameter.");

            string text = "";
            switch (applicationType)
            {
                case ApplicationType.Haftdong:
                    text = "رمز تنظیم مجدد کلمه عبور در هفت دنگ دات کام: " + passwordResetRequest.PasswordResetToken;
                    break;
                case ApplicationType.Sheshdong:
                    text = "رمز تنظیم مجدد کلمه عبور در شیش دنگ دات کام: " + passwordResetRequest.PasswordResetToken;
                    break;
            }


            var result = SmsTransmitter.Send(passwordResetRequest.ContactMethod.NormalizedValue, text);
            if (!result.IsValid)
                return result;

            return ValidationResult.Success;
        }

        #endregion
    }
}