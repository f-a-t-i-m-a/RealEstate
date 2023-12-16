using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compositional.Composer;
using JahanJooy.Common.Util.Localization;
using JahanJooy.RealEstateAgency.Domain.Messages;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.DomainExtensions;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.Notification
{
    public class SmsMessageUtil
    {

        #region Constants

        private const int MaxTransmissionBatchSize = 50;
        private const int MaxDeliveryCheckBatchSize = 50;
        private const int MaxRetryCount = 3;

        private static readonly NotificationReason[] RetryableReasonsWhenNotDelivered =
        {
            NotificationReason.UserRegistered,
        };

        private static readonly NotificationReason[] RetryableReasonsWhenDeliveryUnknown =
        {

        };

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public static DbManager DbManager { get; set; }

        [ComponentPlug]
        public ISmsTransmitter SmsTransmitter { get; set; }

        #endregion

        #region ISmsMessageService implementation

        public static void EnqueueOutgoingMessage(string messageText, string targetNumber, NotificationReason reason,
          NotificationSourceEntityType sourceEntityType, ObjectId sourceEntityId, bool isFlash = false, ObjectId? targetUserId = null,
          DateTime? scheduledDate = null, DateTime? expirationDate = null, bool allowTransmissionOnAnyTimeOfDay = false)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                throw new ArgumentNullException("messageText");

            var validatedTargetNumber = LocalPhoneNumberUtils.ValidateAndFormat(targetNumber, true, true);
            if (!validatedTargetNumber.IsValid)
                throw new ArgumentException("Target number " + targetNumber + " is not valid: " +
                                            string.Join(", ", validatedTargetNumber.Errors.Select(e => e.ErrorKey)));

            targetNumber = validatedTargetNumber.Result.Trim().Replace(" ", "");

            var message = new OutgoingSmsMessage
            {
                CreationDate = DateTime.Now,
                TargetUserID = targetUserId,
                ScheduledDate = scheduledDate,
                ExpirationDate = expirationDate,
                AllowTransmissionOnAnyTimeOfDay = allowTransmissionOnAnyTimeOfDay,
                Reason = reason,
                State = OutgoingSmsMessageState.InQueue,
                StateDate = DateTime.Now,
                SourceEntityType = sourceEntityType,
                SourceEntityID = sourceEntityId,
                RetryIndex = 0,
                RetryForMessageID = null,
                TargetNumber = targetNumber,
                MessageText = messageText,
                IsFlash = isFlash,
                SenderNumber = null,
                ErrorCode = null,
                LastDeliveryCode = null,
                OperatorAssignedID = null
            };

            DbManager.OutgoingSmsMessage.InsertOneAsync(message).Wait();
            //            DbManager.Db.OutgoingSmsMessagesDbSet.Add(message);
        }

    

        #endregion
    }
}
