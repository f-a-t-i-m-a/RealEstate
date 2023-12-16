using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.EF;
using JahanJooy.Common.Util.Localization;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Messages;
using log4net;
using log4net.Util;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class SmsMessageService : ISmsMessageService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SmsMessageService));

        #region Constants

        private const int MaxTransmissionBatchSize = 50;
        private const int MaxDeliveryCheckBatchSize = 50;
        private const int MaxRetryCount = 3;

        private static readonly NotificationReason[] RetryableReasonsWhenNotDelivered =
        {
            NotificationReason.UserRegistered,
            NotificationReason.LoginNameQuery,
            NotificationReason.ListingRegistered,
            NotificationReason.ListingAboutToExpire,
            NotificationReason.NewSavedSearchResult,
			NotificationReason.PaymentReceived,
            NotificationReason.BalanceRunningLow,
            NotificationReason.BalanceDepleted,
            NotificationReason.OperatorRequest,
            NotificationReason.Advertisement
        };

        private static readonly NotificationReason[] RetryableReasonsWhenDeliveryUnknown =
        {
            NotificationReason.ListingAboutToExpire,
			NotificationReason.PaymentReceived,
            NotificationReason.BalanceRunningLow,
            NotificationReason.BalanceDepleted,
            NotificationReason.OperatorRequest
        };

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ISmsTransmitter SmsTransmitter { get; set; }

        #endregion

        #region ISmsMessageService implementation

        public void EnqueueOutgoingMessage(string messageText, string targetNumber, NotificationReason reason,
            NotificationSourceEntityType sourceEntityType, long sourceEntityId, bool isFlash = false, long? targetUserId = null,
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

            Log.DebugFormat("Message enqueued for {0} {1} with reason {2}, target: '{3}', length: {4}",
                sourceEntityType, sourceEntityId, reason, targetNumber, messageText.Length);

            DbManager.Db.OutgoingSmsMessagesDbSet.Add(message);
        }

        public void ResetOutgoingMessage(long messageId)
        {
            var message = DbManager.Db.OutgoingSmsMessagesDbSet.SingleOrDefault(m => m.ID == messageId);
            if (message == null)
                return;

            if (message.ExpirationDate.HasValue && message.ExpirationDate.Value < DateTime.Now)
                return;

            if (message.State == OutgoingSmsMessageState.Transmitting || message.State == OutgoingSmsMessageState.ErrorInTransmission)
                message.SetState(OutgoingSmsMessageState.InQueue);
        }

        public void CancelOutgoingMessage(long messageId)
        {
            var message = DbManager.Db.OutgoingSmsMessagesDbSet.SingleOrDefault(m => m.ID == messageId);
            if (message == null)
                return;

            if (message.ExpirationDate.HasValue && message.ExpirationDate.Value < DateTime.Now)
                return;

            if (message.State == OutgoingSmsMessageState.InQueue || message.State == OutgoingSmsMessageState.Transmitting ||
                message.State == OutgoingSmsMessageState.AwaitingDelivery)
            {
                message.ExpirationDate = DateTime.Now;
            }
        }

        public bool RunTransmissionBatch()
        {
            Log.Debug("Starting transmission batch");
            var db = DbManager.GetFreshDb();

            var messages = LoadMessageBatchForTransmission(db);
            Log.DebugFormat("Transmission batch size: {0}", messages.Count);

            if (!messages.Any())
                return false;

            Log.DebugExt(() => "Transmission batch contains messages " + string.Join(", ", messages.Select(m => m.ID.ToString(CultureInfo.InvariantCulture))));

            messages.SetState(OutgoingSmsMessageState.Transmitting);
            DbManager.SaveDbChanges(db);

            SmsTransmitter.SendBatch(messages);
            Log.Debug("SmsTransmitter.SendBatch returned.");

            UpdateTransmittedMessageStates(messages);

            var retryMessages = BuildRetryMessagesAfterTransmission(messages);
            Log.DebugFormat("Transmission determined to retry {0} messages", retryMessages.Count);
            db.OutgoingSmsMessagesDbSet.AddAll(retryMessages);

            Log.Debug("End of transmission batch");
            return messages.Count >= MaxTransmissionBatchSize;
        }

        public bool RunDeliveryCheckingBatch()
        {
            Log.Debug("Starting check delivery batch");

            var messages = LoadMessageBatchForDeliveryChecking(DbManager.Db);
            Log.DebugFormat("Check delivery batch size: {0}", messages.Count);

            if (!messages.Any())
                return false;

            Log.DebugExt(() => "Check delivery batch contains messages " + string.Join(", ", messages.Select(m => m.ID.ToString(CultureInfo.InvariantCulture))));

            SmsTransmitter.CheckDelivery(messages);
            Log.Debug("SmsTransmitter.CheckDelivery returned.");

            UpdateDeliveryCheckedMessageStates(messages);

            var retryMessages = BuildRetryMessagesAfterDeliveryCheck(messages);
            Log.DebugFormat("Check delivery determined to retry {0} messages", retryMessages.Count);
            DbManager.Db.OutgoingSmsMessagesDbSet.AddAll(retryMessages);

            Log.Debug("End of check delivery batch");
            return messages.Count >= MaxDeliveryCheckBatchSize;
        }

        #endregion

        #region Private helper methods

        private List<OutgoingSmsMessage> LoadMessageBatchForTransmission(Db db)
        {
            var now = DateTime.Now;

            var messageQuery = db.OutgoingSmsMessagesDbSet.Where(m =>
                (m.State == OutgoingSmsMessageState.InQueue) &&
                (m.ScheduledDate == null || m.ScheduledDate < now) &&
                (m.ExpirationDate == null || m.ExpirationDate > now));

            if (!IsAllowedTimeForMessageTransmission(now))
                messageQuery = messageQuery.Where(m => m.AllowTransmissionOnAnyTimeOfDay);

            return messageQuery
                .Include(m => m.RetryForMessage)
                .OrderBy(m => m.CreationDate)
                .Take(MaxTransmissionBatchSize)
                .ToList();
        }

        private List<OutgoingSmsMessage> LoadMessageBatchForDeliveryChecking(Db db)
        {
            var now = DateTime.Now;
            var anHourAgo = now.AddHours(-1);

            var messageQuery = db.OutgoingSmsMessagesDbSet.Where(m =>
                (m.State == OutgoingSmsMessageState.AwaitingDelivery) &&
                (m.StateDate < anHourAgo));

            return messageQuery
                .Include(m => m.RetryForMessage)
                .OrderBy(m => m.StateDate)
                .Take(MaxDeliveryCheckBatchSize)
                .ToList();
        }

        private static bool IsAllowedTimeForMessageTransmission(DateTime now)
        {
            return now.Hour >= 9 && now.Hour < 21;
        }

        private List<OutgoingSmsMessage> BuildRetryMessagesAfterTransmission(IEnumerable<OutgoingSmsMessage> messages)
        {
            var result = new List<OutgoingSmsMessage>();

            foreach (var message in messages.Where(m => m.State == OutgoingSmsMessageState.ErrorInTransmission))
            {
                // Put a global maximum number of retries to prevent unlimited retries in case
                // there is a problem with the code or the SMS operator

                if (message.RetryIndex >= MaxRetryCount)
                    continue;

                // Query the transmitter if the failure is applicable for retry, and how long
                // does it think (regarding the error type) should we wait before retrying.

                TimeSpan? retryDelay = SmsTransmitter.GetRetryDelay(message);

                // If the transmitter suggests that this failure is not retryable,
                // do not retry and leave the failed message.

                if (!retryDelay.HasValue)
                    continue;

                Log.DebugFormat("Retrying OutgoingSmsMessage {0} after error in transmission", message.ID);
                result.Add(BuildRetryMessage(message, retryDelay.Value));
            }

            return result;
        }

        private List<OutgoingSmsMessage> BuildRetryMessagesAfterDeliveryCheck(IEnumerable<OutgoingSmsMessage> messages)
        {
            var result = new List<OutgoingSmsMessage>();

            foreach (var message in messages)
            {
                // Put a global maximum number of retries to prevent unlimited retries in case
                // there is a problem with the code or the SMS operator
                if (message.RetryIndex >= MaxRetryCount)
                    continue;

                // If the message is delivered or it is still waiting for delivery check later, don't retry.
                if (message.State != OutgoingSmsMessageState.NotDelivered &&
                    message.State != OutgoingSmsMessageState.DeliveryUnknown)
                    continue;
                
                // If the message is already expired, don't bother retrying.
                if (message.ExpirationDate.HasValue && message.ExpirationDate.Value <= DateTime.Now)
                    continue;

                // Retry based on the message reason

                if (message.State == OutgoingSmsMessageState.DeliveryUnknown &&
                    RetryableReasonsWhenDeliveryUnknown.Any(r => r == message.Reason))
                {
                    Log.DebugFormat("Retrying OutgoingSmsMessage {0} because the delivery is unknown", message.ID);
                    result.Add(BuildRetryMessage(message, TimeSpan.Zero));
                }

                if (message.State == OutgoingSmsMessageState.NotDelivered &&
                    RetryableReasonsWhenNotDelivered.Any(r => r == message.Reason))
                {
                    Log.DebugFormat("Retrying OutgoingSmsMessage {0} because it is not delivered", message.ID);
                    result.Add(BuildRetryMessage(message, TimeSpan.Zero));
                }
            }

            return result;
        }

        private OutgoingSmsMessage BuildRetryMessage(OutgoingSmsMessage message, TimeSpan retryDelay)
        {
            return new OutgoingSmsMessage
            {
                CreationDate = DateTime.Now,
                TargetUserID = message.TargetUserID,
                ScheduledDate = DateTime.Now + retryDelay,
                ExpirationDate = message.ExpirationDate,
                AllowTransmissionOnAnyTimeOfDay = message.AllowTransmissionOnAnyTimeOfDay,
                Reason = message.Reason,
                State = OutgoingSmsMessageState.InQueue,
                StateDate = DateTime.Now,
                SourceEntityType = message.SourceEntityType,
                SourceEntityID = message.SourceEntityID,
                RetryIndex = message.RetryIndex + 1,
                RetryForMessageID = message.RetryForMessageID.HasValue ? message.RetryForMessageID.Value : message.ID,
                TargetNumber = message.TargetNumber,
                MessageText = message.MessageText,
                IsFlash = message.IsFlash
            };
        }

        private void UpdateTransmittedMessageStates(IEnumerable<OutgoingSmsMessage> messages)
        {
            foreach (var message in messages)
            {
                if (!message.TransmissionDate.HasValue)
                    message.TransmissionDate = DateTime.Now;

                // If the transmitter has set a correct state, leave it.
                if (message.State == OutgoingSmsMessageState.InQueue || message.State == OutgoingSmsMessageState.Transmitting)
                {
                    if (message.ErrorCode.HasValue || string.IsNullOrWhiteSpace(message.OperatorAssignedID))
                        message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
                    else
                        message.SetState(OutgoingSmsMessageState.AwaitingDelivery);
                }

                Log.DebugFormat("OutgoingSmsMessage {0} => State {1}", message.ID, message.State);
            }
        }

        private void UpdateDeliveryCheckedMessageStates(IEnumerable<OutgoingSmsMessage> messages)
        {
            foreach (var message in messages)
            {
                // If the state has already been changed from awaiting delivery, leave the message as it is
                if (message.State == OutgoingSmsMessageState.AwaitingDelivery)
                {
                    // If the message is not known to be delivered after three days, consider its delivery unknown.
                    if (!message.TransmissionDate.HasValue || message.TransmissionDate.Value.AddDays(3) < DateTime.Now)
                        message.SetState(OutgoingSmsMessageState.DeliveryUnknown);
                }

                Log.DebugFormat("OutgoingSmsMessage {0} => State {1}", message.ID, message.State);
            }
        }

        #endregion
    }
}