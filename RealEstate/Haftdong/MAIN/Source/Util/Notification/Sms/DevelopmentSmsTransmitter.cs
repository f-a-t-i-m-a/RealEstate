using System;
using System.Globalization;
using System.IO;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Util.Enums;
using log4net;

namespace JahanJooy.RealEstateAgency.Util.Notification.Sms
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class DevelopmentSmsTransmitter : ISmsTransmitter, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DevelopmentSmsTransmitter));
        private const string AppSettingKeyForOutputDirectory = "DevelopmentSmsTransmitter.OutputDirectory";

        static DevelopmentSmsTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForOutputDirectory);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #region ISmsTransmitter implementation

        public ValidationResult Send(string contactMethodText, string text)
        {
            try
            {
                int index = 1;
                var ticks = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
            
                string path = Path.Combine(ApplicationSettings[AppSettingKeyForOutputDirectory] ?? "C:\\",
                    ticks + "." + index.ToString(CultureInfo.InvariantCulture) + ".sms");

                string fileContents = "Recipient: " + contactMethodText + Environment.NewLine +
                                      "Message: " + text;

                File.WriteAllText(path, fileContents);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in sending verification sms(s)", e);
            }
            return ValidationResult.Success;
        }

        #endregion
    }
}