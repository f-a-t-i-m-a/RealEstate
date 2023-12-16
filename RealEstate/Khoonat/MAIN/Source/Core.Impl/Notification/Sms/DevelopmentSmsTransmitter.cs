using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Sms
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class DevelopmentSmsTransmitter : ISmsTransmitter, IEagerComponent
    {
        private const string AppSettingKeyForOutputDirectory = "DevelopmentSmsTransmitter.OutputDirectory";

        static DevelopmentSmsTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForOutputDirectory);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #region ISmsTransmitter implementation

        public void SendBatch(List<OutgoingSmsMessage> messages)
        {
            int index = 1;
            var ticks = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

            foreach (var message in messages)
            {
                string path = Path.Combine(ApplicationSettings[AppSettingKeyForOutputDirectory] ?? "C:\\",
                    ticks + "." + index.ToString(CultureInfo.InvariantCulture) + ".sms");

                string fileContents = "Recipient: " + message.TargetNumber + Environment.NewLine +
                                      "Message: " + message.MessageText;

                File.WriteAllText(path, fileContents);
                message.State = OutgoingSmsMessageState.AwaitingDelivery;
                index++;
            }
        }

        public void CheckDelivery(List<OutgoingSmsMessage> messages)
        {
            var random = new Random();

            foreach (var message in messages)
            {
                // Consider 50% of the messages as if their delivery is not yet known.
                if (random.Next(100) < 50)
                {
                    message.StateDate = DateTime.Now;
                    continue;
                }

                // Consider 80% of the rest of the messages as delivered.
                if (random.Next(100) < 80)
                {
                    message.SetState(OutgoingSmsMessageState.Delivered);
                    continue;
                }

                // Consider half of the rest of the messages as not delivered, and the other half unknown.
                message.SetState(random.Next(100) < 50 ? 
                    OutgoingSmsMessageState.NotDelivered : 
                    OutgoingSmsMessageState.DeliveryUnknown);
            }
        }

        public TimeSpan? GetRetryDelay(OutgoingSmsMessage message)
        {
            return null;
        }

        #endregion
    }
}