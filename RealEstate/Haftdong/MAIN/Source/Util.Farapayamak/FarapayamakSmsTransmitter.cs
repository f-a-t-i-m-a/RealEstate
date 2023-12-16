using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Util.Farapayamak.ServiceReferenceFarapayamak;
using JahanJooy.RealEstateAgency.Util.Notification;
using log4net;

namespace JahanJooy.RealEstateAgency.Util.Farapayamak
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class FarapayamakSmsTransmitter : ISmsTransmitter, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FarapayamakSmsTransmitter));

        #region ApplicationSetting constants

        private const string SettingKeyForUsername = "Farapayamak.Username";
        private const string SettingKeyForPassword = "Farapayamak.Password";
        private const string SettingKeyForFrom = "Farapayamak.From";

        private readonly List<string> _errorKeys = new List<string>
        {
            "0",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"
        };

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #endregion

        static FarapayamakSmsTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(SettingKeyForUsername);
            ApplicationSettingKeys.RegisterKey(SettingKeyForPassword);
            ApplicationSettingKeys.RegisterKey(SettingKeyForFrom);
        }

        #region Implementation of ISmsTransmitter

        public ValidationResult Send(string contactMethodText, string text)
        {
            if (text.IsNullOrWhitespace())
            {
                Log.Error("An error has been occured in sending verification sms(s), text is empty");
                return ValidationResult.Success;
            }

            var username = ApplicationSettings[SettingKeyForUsername];
            var password = ApplicationSettings[SettingKeyForPassword];
            var from = ApplicationSettings[SettingKeyForFrom];
            var to = contactMethodText;
            try
            {
                var result = new SendSoapClient().SendSimpleSMS2(username, password, to, from, text, false);
                if (_errorKeys.Any(e => e.Equals(result)))
                {
                    Log.ErrorFormat("An exception has been occured in sending verification sms(s) with result {0}",
                        result);
                }
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