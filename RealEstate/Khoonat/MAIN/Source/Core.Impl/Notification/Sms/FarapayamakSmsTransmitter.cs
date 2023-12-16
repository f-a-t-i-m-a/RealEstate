using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Wcf;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.ExternalServices.Farapayamak.ActionsWebSerivce;
using JahanJooy.RealEstate.Core.Impl.ExternalServices.Farapayamak.AlertsWebService;
using JahanJooy.RealEstate.Core.Impl.ExternalServices.Farapayamak.SendWebService;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Domain.Messages;
using JahanJooy.RealEstate.Util.Log4Net;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Sms
{
	[Component]
	[IgnoredOnAssemblyRegistration]
	public class FarapayamakSmsTransmitter : ISmsTransmitter, IEagerComponent
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (FarapayamakSmsTransmitter));

	    #region ApplicationSetting constants

	    private const string SettingKeyForUsername = "Farapayamak.Username";
	    private const string SettingKeyForPassword = "Farapayamak.Password";

	    #endregion

	    #region Error code constants

	    private const int ErrorCodeForServiceCallException = -1000;
	    private const int ErrorCodeForEmptyResultFromWebService = -2000;
	    private const int ErrorCodeForTransmissionStatusNotReported = -2001;
	    private const int ErrorCodeForUnsupportedStatus = -2002;
        private const int ErrorCodeForNoSenderNumberAvailableForMessage = -2003;

	    private const int ErrorCodeForWrongUsernameOrPassword = 0;
	    private const int ErrorCodeForLowCreditBalance = 2;
	    private const int ErrorCodeForReachedDailyLimit = 3;
	    private const int ErrorCodeForReachedSizeLimit = 4;
	    private const int ErrorCodeForWrongSenderNumber = 5;
	    private const int ErrorCodeForSystemBeingUpgraded = 6;
	    private const int ErrorCodeForTextContainingFilteredWords = 7;
	    private const int ErrorCodeForNotMeetingMinimumTxLimit = 8;
	    private const int ErrorCodeForPublicNumbersDeniedViaWebService = 9;
	    private const int ErrorCodeForUserIsBlocked = 10;
	    private const int ErrorCodeForBlockedSender = 11;

	    private const byte DeliveryCodeForEmptyResultFromWebService = 250;
	    private const byte DeliveryCodeForServiceCallException = 251;

	    private const byte DeliveryCodeForSentToTelecom = 0;
	    private const byte DeliveryCodeForDeliveredToHandset = 1;
	    private const byte DeliveryCodeForNotDeliveredToHandset = 2;
	    private const byte DeliveryCodeForCommunicationError = 3;
	    private const byte DeliveryCodeForUnknownError = 5;
	    private const byte DeliveryCodeForReachedTelecom = 8;
	    private const byte DeliveryCodeForNotReachedTelecom = 16;
	    private const byte DeliveryCodeForUnknown = 100;


	    private static readonly string[] BlockableSenderPrefixes = {"1000", "2000", "3000", "5000"};

	    #endregion

	    #region Injected dependencies

	    [ComponentPlug]
	    public ChannelFactoryCache ChannelFactories { get; set; }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

	    #endregion

        #region Fields

	    private static readonly FarapayamakSenderLineStatus[] LineStatusesAllowedToSend =
	    {
	        FarapayamakSenderLineStatus.Healthy,
	        FarapayamakSenderLineStatus.HasDelaysInDeliveryReport,
	        FarapayamakSenderLineStatus.HasDeliveryReportIssues,
	        FarapayamakSenderLineStatus.HasHighTransmissionTraffic,
	        FarapayamakSenderLineStatus.HasDelaysInTransmission,
            FarapayamakSenderLineStatus.Unknown
	    };

	    private FarapayamakSenderLinesCache _senderLines;
        private readonly object _senderLinesLock = new object();

        #endregion

	    static FarapayamakSmsTransmitter()
	    {
	        ApplicationSettingKeys.RegisterKey(SettingKeyForUsername);
	        ApplicationSettingKeys.RegisterKey(SettingKeyForPassword);

	        FarapayamakSenderLinesCache.RegisterConfigurationKeys();
            ChannelFactoryCache.RegisterConfigurationKeys(typeof(SendSoap));
            ChannelFactoryCache.RegisterConfigurationKeys(typeof(ActionsSoap));
            ChannelFactoryCache.RegisterConfigurationKeys(typeof(AlertsSoap));
	    }

	    #region Implementation of ISmsTransmitter

	    public void SendBatch(List<OutgoingSmsMessage> messages)
	    {
	        if (messages == null || messages.Count < 1)
	            return;

            Log.DebugFormat("Starting SendBatch with a set of {0} messages", messages.Count);

	        // Set sender number on all messages

	        var lines = GetSenderLines();
	        foreach (var message in messages)
	            message.SenderNumber = DetermineSenderNumberFor(message, lines);

	        // Mark any message that there is no sender number available for it as an error,
	        // and filter them out for the rest of the procedure.

	        foreach (var message in messages.Where(m => string.IsNullOrWhiteSpace(m.SenderNumber)))
	        {
	            Log.WarnFormat("Could not choose an appropriate sender number for Outgoing SMS {0} retry index {1} with target number {2}", 
                    message.ID, message.RetryIndex, message.TargetNumber);

	            message.ErrorCode = ErrorCodeForNoSenderNumberAvailableForMessage;
	            message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	        }

	        messages = messages.Where(m => !string.IsNullOrWhiteSpace(m.SenderNumber)).ToList();

	        // Since the webservice doesn't allow us to specify isFlash
	        // for each message, and a single isFlash value is used for
	        // a whole batch, we group messages by having IsFlash property
	        // true or false, and send each group separately.

	        var messageGroups = messages.GroupBy(m => m.IsFlash);
	        foreach (var messageGroup in messageGroups)
	        {
	            var isFlash = messageGroup.Key;
	            var messagesToTransmit = messageGroup.ToList();
	            if (!messagesToTransmit.Any())
	                continue;

	            try
	            {
	                ChannelFactories.Use<ActionsSoap>(service =>
	                {
                        Log.DebugFormat("About to send a WS request to send {0} messages with isFlash={1}", messagesToTransmit.Count, isFlash);
                        RealEstateStaticLogs.Farapayamak.InfoFormat("SendMultipleSMS2 - Calling for {0} messages: {1}",
                            messagesToTransmit.Count, string.Join(",", messagesToTransmit.Select(m => m.ID.ToString(CultureInfo.InvariantCulture))));

                        var request = BuildRequestForSendMultiple(messagesToTransmit, isFlash);
	                    var response = service.SendMultipleSMS2(request);

	                    if (response == null || response.Body == null)
	                    {
                            Log.Warn("WS invokation for SendMultipleSMS2 returned empty result.");
                            RealEstateStaticLogs.Farapayamak.WarnFormat("SendMultipleSMS2 - Returned empty result");

	                        messagesToTransmit.ForEach(m => { m.ErrorCode = ErrorCodeForEmptyResultFromWebService; });
	                        messagesToTransmit.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	                        return;
	                    }

	                    RealEstateStaticLogs.Farapayamak.Info("SendMultipleSMS2 - Results  = " + string.Join(", ",
	                        (response.Body.SendMultipleSMS2Result ?? Enumerable.Empty<int>()).Select(i => i.ToString(CultureInfo.InvariantCulture))));
	                    RealEstateStaticLogs.Farapayamak.Info("SendMultipleSMS2 - Statuses = " + string.Join(", ",
	                        (response.Body.status ?? Enumerable.Empty<byte>()).Select(b => b.ToString(CultureInfo.InvariantCulture))));

	                    for (int i = 0; i < messagesToTransmit.Count; i++)
	                    {
	                        var message = messagesToTransmit[i];
	                        var result = (response.Body.SendMultipleSMS2Result != null &&
	                                      response.Body.SendMultipleSMS2Result.Count > i)
	                            ? (int?) response.Body.SendMultipleSMS2Result[i]
	                            : null;
	                        var status = (response.Body.status != null && response.Body.status.Length > i)
	                            ? (byte?) response.Body.status[i]
	                            : null;
	                        var recId = (response.Body.recId != null && response.Body.recId.Count > i)
	                            ? (long?) response.Body.recId[i]
	                            : null;

	                        UpdateMessageFromTransmissionServiceResult(result, message, status, recId);
	                    }
	                });
	            }
	            catch (Exception e)
	            {
                    Log.Error("Exception during SendMultipleSMS2 WS call and processing result", e);

                    messagesToTransmit.ForEach(m => { m.ErrorCode = ErrorCodeForServiceCallException; });
	                messagesToTransmit.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	            }
	        }
	    }

	    public void CheckDelivery(List<OutgoingSmsMessage> messages)
	    {
            if (messages == null || messages.Count < 1)
                return;

            Log.DebugFormat("Starting CheckDelivery with a set of {0} messages", messages.Count);

	        var messagesToCheck = messages.Where(m => !string.IsNullOrWhiteSpace(m.OperatorAssignedID)).ToList();

            Log.DebugFormat("About to send a WS request to check delivery of {0} messages", messagesToCheck.Count);
            RealEstateStaticLogs.Farapayamak.InfoFormat("GetMultiDelivery - Calling for {0} messages: {1}",
                messagesToCheck.Count, string.Join(",", messagesToCheck.Select(m => m.ID.ToString(CultureInfo.InvariantCulture))));

            try
            {
                ChannelFactories.Use<SendSoap>(service =>
                {

                    // TODO: Uncomment and fix the code to use GetMultiDelivery when the webservice is fixed from Farapayamak's side.

//                    var request = BuildRequestForGetMultiDelivery(messagesToCheck);
//                    var response = service.GetMultiDelivery(request);

//                    if (response == null || response.Body == null)
//                    {
//                        Log.Warn("WS invokation for GetMultiDelivery returned empty result.");
//                        RealEstateStaticLogs.Farapayamak.WarnFormat("GetMultiDelivery - Returned empty result");

//                        messagesToCheck.ForEach(m =>
//                        {
//                            m.LastDeliveryCode = DeliveryCodeForEmptyResultFromWebService;
//                            m.StateDate = DateTime.Now;
//                        });
//                        return;
//                    }

//                    RealEstateStaticLogs.Farapayamak.Info("GetMultiDelivery - Results  = " + string.Join(", ",
//                        (response.Body.GetMultiDeliveryResult ?? Enumerable.Empty<byte>()).Select(b => b.ToString(CultureInfo.InvariantCulture))));

                    for (int i = 0; i < messagesToCheck.Count; i++)
                    {
                        var message = messagesToCheck[i];
//                        var result = (response.Body.GetMultiDeliveryResult != null &&
//                                      response.Body.GetMultiDeliveryResult.Length > i)
//                            ? (byte?)response.Body.GetMultiDeliveryResult[i]
//                            : null;

                        // TODO: Temporary code start - workaround Farapayamak's WS issue with GetMultiDelivery

                        long recId;
                        int? result;
                        if (!long.TryParse(message.OperatorAssignedID, out recId))
                        {
                            Log.Warn("Unable to check delivery of message " + message.ID + " which does not have an appropriate OperatorAssignedID.");
                            result = null;
                        }
                        else
                        {
                            RealEstateStaticLogs.Farapayamak.InfoFormat("GetDelivery - Calling for message {0}, recID: {1}", message.ID, recId);
                            result = service.GetDelivery(recId);
                            RealEstateStaticLogs.Farapayamak.InfoFormat("GetDelivery - Result = {0}", result);
                        }

                        // TODO: Temporary code end

                        UpdateMessageFromDeliveryServiceResult(result, message);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Exception during GetMultiDelivery WS call and processing result", e);

                messagesToCheck.ForEach(m => { m.LastDeliveryCode = DeliveryCodeForServiceCallException; });
                messagesToCheck.SetState(OutgoingSmsMessageState.AwaitingDelivery);
            }
	    }

	    public TimeSpan? GetRetryDelay(OutgoingSmsMessage message)
	    {
	        if (message == null || message.ErrorCode == null)
	        {
	            // Unknown error, do not retry
	            return null;
	        }

	        // If the sender number is blocked by receiver (n000 numbers), and we have a non-blockable 
	        // number to send from, retry quickly.
	        // But if the sender is not blockable and we get this error, there's nothing we can do.
	        // So don't retry at all.

	        if (message.ErrorCode.Value == ErrorCodeForBlockedSender)
	        {
	            if (BlockableSenderPrefixes.Any(p => message.SenderNumber.StartsWith(p)))
	            {
                    // Hopefully we do have a healthy number that is not blockable
                    Log.DebugFormat("Outgoing SMS {0} is blocked because of sender number. Can retry immediately.", message.ID);
	                return TimeSpan.Zero;
	            }

                Log.DebugFormat("Outgoing SMS {0} is blocked from non-blockable number. Cannot retry.", message.ID);
                return null;
	        }

            // If there's a problem with web service calls or interpretation of the results,
            // the most possible reason is that there is a network problem or a system upgrade
            // in progress on the Farapayamak's site. So retry in an hour and hope the issue
            // won't happen again.

	        if (message.ErrorCode.Value == ErrorCodeForServiceCallException || 
                message.ErrorCode.Value == ErrorCodeForEmptyResultFromWebService ||
                message.ErrorCode.Value == ErrorCodeForTransmissionStatusNotReported ||
                message.ErrorCode.Value == ErrorCodeForUnsupportedStatus)
	        {
                Log.DebugFormat("Outgoing SMS {0} has invocation issues, can retry in an hour.", message.ID);
	            return TimeSpan.FromHours(1);
	        }

	        // If the error is an administrative one (where a human-interaction is required to fix it - 
	        // such as wrong username / password, account being blocked, or low credit balance), retry 
	        // regarding the expiration time, or some default long duration.

	        if (message.ErrorCode.Value == ErrorCodeForWrongUsernameOrPassword ||
	            message.ErrorCode.Value == ErrorCodeForLowCreditBalance || 
	            message.ErrorCode.Value == ErrorCodeForWrongSenderNumber || 
	            message.ErrorCode.Value == ErrorCodeForPublicNumbersDeniedViaWebService ||
	            message.ErrorCode.Value == ErrorCodeForUserIsBlocked ||
                message.ErrorCode.Value == ErrorCodeForNoSenderNumberAvailableForMessage)
	        {
                Log.DebugFormat("Outgoing SMS {0} has administrative issues, can retry in a maximum of two hour.", message.ID);
                
                if (message.ExpirationDate.HasValue)
	            {
	                // If the message is already expired, do not retry.

	                if (message.ExpirationDate.Value < DateTime.Now)
	                    return null;

	                // Calculate half of the time span remaining till the message is expired.

	                var halfRemainingValidity = TimeSpan.FromTicks((message.ExpirationDate.Value - DateTime.Now).Ticks/2);

	                // If it is larger than two hours, retry in two hours.
	                // Hopefully, an administrator will address the problem till then.

	                if (halfRemainingValidity.TotalHours > 2)
	                    return TimeSpan.FromHours(2);

	                // Otherwise, retry when half of the validity period is passed.
	                return halfRemainingValidity;
	            }

	            // There's no expiration, so we don't have a clue weather to rush it or not.
	            // Just retry in two hours, and hope that an administrator has done something till then.

	            return TimeSpan.FromHours(2);
	        }

	        // In case of errors that should not normally happen, just return a time span to
	        // retry. It might be an error from the operator side, or an undocumented limitation.

	        if (message.ErrorCode.Value == ErrorCodeForReachedDailyLimit || 
	            message.ErrorCode.Value == ErrorCodeForReachedSizeLimit || 
	            message.ErrorCode.Value == ErrorCodeForNotMeetingMinimumTxLimit) // Doesn't meet minimum transmission limit
	        {
	            Log.WarnFormat("Outgoing SMS {0} has a transmission error code of {1} which should not normally happen. Attempting retry in 6 hours.",
	                message.ID, message.ErrorCode.Value);
	            return TimeSpan.FromHours(6);
	        }

	        // If the target system is being upgraded, hopefully it will be up and running soon.
	        // So retry in an hour.

	        if (message.ErrorCode.Value == ErrorCodeForSystemBeingUpgraded)
	        {
                Log.DebugFormat("Outgoing SMS {0} can be retried in an hour, a target system upgrade is in progress.", message.ID);
	            return TimeSpan.FromHours(1);
	        }

	        // The filtered content error (which should not normally happen) is not recoverable,
	        // because the content won't change in retry. So choose not to retry in this case.

	        if (message.ErrorCode.Value == ErrorCodeForTextContainingFilteredWords)
	        {
                Log.WarnFormat("Outgoing SMS {0} is claimed to contain filtered words. This should not ever happen. Cannot retry.", message.ID);
	            return null;
	        }

	        // If none of the above cases match, do not retry.
	        Log.WarnFormat("Outgoing SMS {0} has transmission error {1} which is not an identified case. Cannot retry.",
	            message.ID, message.ErrorCode.Value);

	        return null;
	    }

	    #endregion
        
        private FarapayamakSenderLinesCache GetSenderLines()
	    {
	        lock (_senderLinesLock)
	        {
	            if (_senderLines != null && !_senderLines.IsExpired)
	                return _senderLines;

                _senderLines = new FarapayamakSenderLinesCache(ApplicationSettings);
	            GetAlertsResponse response = null;

                try
	            {
                    // TODO: Uncomment the following lines after GetAlerts web service is fixed from Farapayamak's side

//	                ChannelFactories.Use<AlertsSoap>(service =>
//	                {
//                        Log.DebugFormat("About to send a WS request to get alerts");
//                        RealEstateStaticLogs.Farapayamak.InfoFormat("GetAlerts - Calling");
//                        
//                        response = service.GetAlerts(new GetAlertsRequest(new GetAlertsRequestBody(
//	                        ApplicationSettings[SettingKeyForUsername],
//	                        ApplicationSettings[SettingKeyForPassword],
//	                        -1))); // TODO: What is location?
//                        
//                        RealEstateStaticLogs.Farapayamak.InfoFormat("GetAlerts - Result: {0} alerts.",
//                            response.IfNotNull(r => r.Body.IfNotNull(b => b.GetAlertsResult.Count)));
//                    });
                }
	            catch (Exception e)
	            {
                    Log.Error("Exception during GetAlerts WS call", e);
                }

                _senderLines.Initialize(response.IfNotNull(r => r.Body));
	            return _senderLines;
	        }
	    }

	    private string DetermineSenderNumberFor(OutgoingSmsMessage message, FarapayamakSenderLinesCache lines)
	    {
            // If this is a retry for a blocked sender number, use 021 line if it is not used already
	        if (message.RetryForMessage != null && 
                message.RetryForMessage.ErrorCode == ErrorCodeForBlockedSender && 
                lines.Has021Line)
	        {
	            if (message.RetryForMessage == null || 
                    !message.RetryForMessage.SenderNumber.Equals(lines.Get021Line.LineNumber))
	            {
	                return lines.Get021Line.LineNumber;
	            }
	        }

            var lineOrder = lines.GetPrioritizedLines(message.TargetNumber, message.IsFlash);

            // If this is a retry, try to avoid using the same number that was used before and failed.
	        if (message.RetryForMessage != null)
	        {
	            lineOrder = lineOrder.Where(l => !l.LineNumber.Equals(message.RetryForMessage.SenderNumber)).ToArray();
	        }

            // Find the healthier, non-blocked configured line in the order of price and use it.
	        return FindHealthierLine(lineOrder).IfNotNull(l => l.LineNumber);
	    }

        private FarapayamakSenderLine FindHealthierLine(FarapayamakSenderLine[] lineOrder)
	    {
            foreach (var status in LineStatusesAllowedToSend)
            {
                var matchedLine = lineOrder.FirstOrDefault(l => l.Status == status);
                if (matchedLine != null)
                    return matchedLine;
            }

            return null;
	    }

	    private static void UpdateMessageFromTransmissionServiceResult(int? result, OutgoingSmsMessage message, byte? status, long? recId)
	    {
	        if (!result.HasValue || result.Value != 1)
	        {
	            message.ErrorCode = result;
	            message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	            return;
	        }

	        if (!status.HasValue)
	        {
	            message.ErrorCode = ErrorCodeForTransmissionStatusNotReported;
	            message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	            return;
	        }

	        if (status.Value == 1)
	        {
	            message.ErrorCode = ErrorCodeForBlockedSender;
	            message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	            return;
	        }

	        if (status.Value != 0)
	        {
	            message.ErrorCode = ErrorCodeForUnsupportedStatus;
	            message.SetState(OutgoingSmsMessageState.ErrorInTransmission);
	            return;
	        }

	        if (!recId.HasValue)
	        {
	            message.SetState(OutgoingSmsMessageState.DeliveryUnknown);
	            return;
	        }

	        message.SetState(OutgoingSmsMessageState.AwaitingDelivery);
	        message.OperatorAssignedID = recId.Value.ToString(CultureInfo.InvariantCulture);
	    }
	    
        private static void UpdateMessageFromDeliveryServiceResult(int? result, OutgoingSmsMessage message)
	    {
            var lastResult = message.LastDeliveryCode;
            message.LastDeliveryCode = result;
            
            // If the result has some kind of errors or its not known, we want to wait for at least 6 hours 
            // and to try fetching the delivery state at least once, to see if the condition is transient or not. 
            // So if after 6 hours, the result is equal to previous result, we conclude that the message delivery is unknown.

            if (!result.HasValue || 
                result.Value == DeliveryCodeForCommunicationError ||
                result.Value == DeliveryCodeForUnknownError ||
                result.Value == DeliveryCodeForNotReachedTelecom ||
                result.Value == DeliveryCodeForUnknown)
            {
                if (!result.HasValue)
                {
                    Log.WarnFormat("Outgoing SMS {0} has an empty delivery status when checked for delivery.", message.ID);
                }

                message.SetState(result != lastResult || (DateTime.Now - message.TransmissionDate.GetValueOrDefault()).TotalHours < 6
                    ? OutgoingSmsMessageState.AwaitingDelivery
                    : OutgoingSmsMessageState.DeliveryUnknown);
                return;
            }

            // If the result is expected to change, just refresh the state date and continue.
	        if (result.Value == DeliveryCodeForSentToTelecom ||
                result.Value == DeliveryCodeForReachedTelecom)
	        {
	            message.SetState(OutgoingSmsMessageState.AwaitingDelivery);
	            return;
	        }

            // If the result is final, set the state accordingly.
	        if (result.Value == DeliveryCodeForDeliveredToHandset ||
                result.Value == DeliveryCodeForNotDeliveredToHandset)
	        {
	            message.SetState(result.Value == DeliveryCodeForDeliveredToHandset
	                ? OutgoingSmsMessageState.Delivered
	                : OutgoingSmsMessageState.NotDelivered);
	            return;
	        }

            // If none of the above cases match, we have an unknown delivery code returned from
            // the web service. Treat it like an error (retry once).

            Log.WarnFormat("Outgoing SMS {0} has an unidentified delivery status: {1}", message.ID, result);

            message.SetState(result != lastResult
                ? OutgoingSmsMessageState.AwaitingDelivery
                : OutgoingSmsMessageState.DeliveryUnknown);
        }

	    private SendMultipleSMS2Request BuildRequestForSendMultiple(List<OutgoingSmsMessage> messages, bool isFlash)
	    {
	        var request = new SendMultipleSMS2Request(new SendMultipleSMS2RequestBody());
	        request.Body.username = ApplicationSettings[SettingKeyForUsername];
            request.Body.password = ApplicationSettings[SettingKeyForPassword];
	        request.Body.from = new ExternalServices.Farapayamak.ActionsWebSerivce.ArrayOfString();
            request.Body.from.AddRange(messages.Select(m => m.SenderNumber));
	        request.Body.to = new ExternalServices.Farapayamak.ActionsWebSerivce.ArrayOfString();
            request.Body.to.AddRange(messages.Select(m => m.TargetNumber));
	        request.Body.text = new ExternalServices.Farapayamak.ActionsWebSerivce.ArrayOfString();
            request.Body.text.AddRange(messages.Select(m => m.MessageText));
	        request.Body.isflash = isFlash;
	        request.Body.udh = string.Empty;
	        return request;
	    }

        private GetMultiDeliveryRequest BuildRequestForGetMultiDelivery(IEnumerable<OutgoingSmsMessage> messages)
        {
            var result = new GetMultiDeliveryRequest(new GetMultiDeliveryRequestBody());
            result.Body.recId = string.Join(",", messages.Select(m => m.OperatorAssignedID));

            return result;
        }
    }
}