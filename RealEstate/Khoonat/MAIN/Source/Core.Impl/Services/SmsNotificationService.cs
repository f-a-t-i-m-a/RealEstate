using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class SmsNotificationService : ISmsNotificationService
    {
        [ComponentPlug]
        public ISmsMessageService SmsMessageService { get; set; }

        #region ISmsNotificationService implementation

        public void SendVerification(User user, UserContactMethod contactMethod,
            UserContactMethodVerification verification)
        {
            if (contactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The verification method is not SMS.");

            string text = "رمز فعال سازی شماره شما در سایت خونه ت دات کام: " + verification.VerificationSecret;
            SmsMessageService.EnqueueOutgoingMessage(
                text, contactMethod.ContactMethodText, NotificationReason.ContactMethodVerification,
                NotificationSourceEntityType.UserContactMethodVerification, verification.ID,
                targetUserId: user.ID,
                allowTransmissionOnAnyTimeOfDay: true,
                expirationDate: verification.ExpirationTime);
        }

        public void SendPasswordReset(User user, PasswordResetRequest passwordResetRequest)
        {
            if (passwordResetRequest.ContactMethod == null)
                throw new ArgumentException("ContactMethod is not filled for passwordResetRequest parameter.");
            if (passwordResetRequest.ContactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The password reset request method is not SMS");

            string text = "رمز تنظیم مجدد کلمه عبور در خونه ت دات کام: " + passwordResetRequest.PasswordResetToken;
            SmsMessageService.EnqueueOutgoingMessage(
                text, passwordResetRequest.ContactMethod.ContactMethodText, NotificationReason.PasswordResetRequest,
                NotificationSourceEntityType.PasswordResetRequest, passwordResetRequest.ID,
                targetUserId: user.ID,
                allowTransmissionOnAnyTimeOfDay: true,
                expirationDate: passwordResetRequest.ExpirationTime);
        }

        public void SendLoginNameQuery(User user, UserContactMethod contactMethod)
        {
            if (contactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The contact method is not SMS.");

            string text = "نام کاربری شما در سایت خونه ت دات کام: " + user.LoginName;
            SmsMessageService.EnqueueOutgoingMessage(
                text, contactMethod.ContactMethodText, NotificationReason.LoginNameQuery,
                NotificationSourceEntityType.UserContactMethod, contactMethod.ID,
                targetUserId: user.ID,
                allowTransmissionOnAnyTimeOfDay: true);
        }

        public void NotifyBonusAwarded(UserContactMethod contactMethod, PromotionalBonus newBonus)
        {
            if (contactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The verification method is not SMS.");

            string text = "در سایت khoonat.com مبلغ " + newBonus.BonusAmount + " ریال به تخفیف اعتبار شما افزوده شد.";
            SmsMessageService.EnqueueOutgoingMessage(
                text, contactMethod.ContactMethodText, NotificationReason.BonusNotification,
                NotificationSourceEntityType.PromotionalBonus, newBonus.ID,
                targetUserId: contactMethod.UserID);
        }

        public void NotifyBalanceDepleted(UserContactMethod contactMethod, NotificationMessage notification)
        {
            if (contactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The verification method is not SMS.");

            string text =
                "کاربر گرامی، اعتبار مالی شما در سایت khoonat.com تموم شده. برای ادامه استفاده از خدمات ویژه با مراجعه به سایت حسابتون رو شارژ کنید.";
            SmsMessageService.EnqueueOutgoingMessage(
                text, contactMethod.ContactMethodText, NotificationReason.BalanceDepleted,
                NotificationSourceEntityType.NotificationMessage, notification.SourceEntityID.GetValueOrDefault(),
                targetUserId: contactMethod.UserID);
        }

        public void NotifyWireTransferPayment(UserContactMethod contactMethod, UserWireTransferPayment payment)
        {
            if (contactMethod.ContactMethodType != ContactMethodType.Phone)
                throw new ArgumentException("The verification method is not SMS.");

            var amount = string.Format("{0:0.#}", payment.Amount);

            string text = "در سایت khoonat.com مبلغ " + amount + " ریال به اعتبار شما افزوده شد.";
            SmsMessageService.EnqueueOutgoingMessage(
                text, contactMethod.ContactMethodText, NotificationReason.PaymentReceived,
                NotificationSourceEntityType.Userpayment, payment.ID,
                targetUserId: contactMethod.UserID);
        }

        #endregion
    }
}