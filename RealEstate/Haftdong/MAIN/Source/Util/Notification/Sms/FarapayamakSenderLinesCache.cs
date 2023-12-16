using System;
using System.Linq;
using System.Text.RegularExpressions;
using JahanJooy.Common.Util.Configuration;

namespace JahanJooy.RealEstateAgency.Util.Notification.Sms
{
    internal class FarapayamakSenderLinesCache
    {
        // ReSharper disable UnusedMember.Local
        private static readonly Regex MciTargetPattern = new Regex(@"^\+9891");
        private static readonly Regex IrancellTargetPattern = new Regex(@"^\+9893[0356789]");
        private static readonly Regex RightelTargetPattern = new Regex(@"^\+9892[01]");
        // ReSharper restore UnusedMember.Local

        #region ApplicationSetting constants

        private const string SettingKeyFor1KLine = "Farapayamak.Lines.1000";
        private const string SettingKeyFor2KLine = "Farapayamak.Lines.2000";
        private const string SettingKeyFor3KLine = "Farapayamak.Lines.3000";
        private const string SettingKeyFor5KLine = "Farapayamak.Lines.5000";
        private const string SettingKeyFor021Line = "Farapayamak.Lines.021";

        private const string SettingKeySuffixForLineNumber = ".Number";
        private const string SettingKeySuffixForLineDefaultPriority = ".Priority";
        private const string SettingKeySuffixForLinePriorityForIrancell = ".Priority.Irancell";
        private const string SettingKeySuffixForLinePriorityForRightel = ".Priority.Rightel";

        #endregion

        #region Provate fields

        private readonly DateTime _expirationTime;

        private readonly FarapayamakSenderLine _1KNumber;
        private readonly FarapayamakSenderLine _2KNumber;
        private readonly FarapayamakSenderLine _3KNumber;
        private readonly FarapayamakSenderLine _5KNumber;
        private readonly FarapayamakSenderLine _021Number;

        #endregion

        #region Initialization API

        public FarapayamakSenderLinesCache(IApplicationSettings settings)
        {
            _1KNumber = CreateLine(settings, SettingKeyFor1KLine, "1000", true);
            _2KNumber = CreateLine(settings, SettingKeyFor2KLine, "2000", true);
            _3KNumber = CreateLine(settings, SettingKeyFor3KLine, "3000", true);
            _5KNumber = CreateLine(settings, SettingKeyFor5KLine, "5000", true);
            _021Number = CreateLine(settings, SettingKeyFor021Line, "21", false);

            // Not initialized yet, so already expired.
            _expirationTime = DateTime.Now;
        }

//        public void Initialize(GetAlertsResponseBody alertsResponseBody)
//        {
//            // Default expiration time is set to 15 minutes, unless we don't have any health information
//            _expirationTime = DateTime.Now + TimeSpan.FromMinutes(alertsResponseBody == null ? 5 : 15);
//
//            // Extract health information from alerts response
//            SetLineHealth(alertsResponseBody);
//
//            // Decrease line statuses to blocked for lines that have time limitations
//            SetBlockedNumbers();
//
//            ApplicationStaticLogs.Farapayamak.InfoFormat("Lines health re-initized. 1000: {0}; 2000: {1}; 3000: {2}; 5000: {3}, 021: {4}",
//                _1KNumber.Status, _2KNumber.Status, _3KNumber.Status, _5KNumber.Status, _021Number.Status);
//        }

        #endregion

        #region Query API

        public bool IsExpired => _expirationTime < DateTime.Now;

        public bool Has1KLine => _1KNumber.Status != FarapayamakSenderLineStatus.NotConfigured;

        public bool Has2KLine => _2KNumber.Status != FarapayamakSenderLineStatus.NotConfigured;

        public bool Has3KLine => _3KNumber.Status != FarapayamakSenderLineStatus.NotConfigured;

        public bool Has5KLine => _5KNumber.Status != FarapayamakSenderLineStatus.NotConfigured;

        public bool Has021Line => _021Number.Status != FarapayamakSenderLineStatus.NotConfigured;

        public FarapayamakSenderLine Get1KLine => _1KNumber;

        public FarapayamakSenderLine Get2KLine => _2KNumber;

        public FarapayamakSenderLine Get3KLine => _3KNumber;

        public FarapayamakSenderLine Get5KLine => _5KNumber;

        public FarapayamakSenderLine Get021Line => _021Number;

        public FarapayamakSenderLine[] GetAllLines()
        {
            return new[] {_1KNumber, _2KNumber, _3KNumber, _5KNumber, _021Number};
        }

        public FarapayamakSenderLine[] GetPrioritizedLines(string targetNumber, bool isFlash)
        {
            var result = GetAllLines().AsEnumerable();

            if (isFlash)
                result = result.Where(l => l.IsFlashCapable);

            if (IrancellTargetPattern.IsMatch(targetNumber))
                result = result.Where(l => l.LinePriorityForIrancell >= 0).OrderByDescending(l => l.LinePriorityForIrancell);
            else if (RightelTargetPattern.IsMatch(targetNumber))
                result = result.Where(l => l.LinePriorityForRightel >= 0).OrderByDescending(l => l.LinePriorityForRightel);
            else
                result = result.Where(l => l.LineDefaultPriority >= 0).OrderByDescending(l => l.LineDefaultPriority);

            return result.Where(l => l.Status != FarapayamakSenderLineStatus.NotConfigured).ToArray();
        }

        #endregion

        #region Private helper methods

        private FarapayamakSenderLine CreateLine(IApplicationSettings settings, string settingKeyPrefix, string prefix, bool isFlashCapable)
        {
            var number = settings[settingKeyPrefix + SettingKeySuffixForLineNumber];
            var defaultPriority = settings.GetInt(settingKeyPrefix + SettingKeySuffixForLineDefaultPriority);
            var irancellPriority = settings.GetInt(settingKeyPrefix + SettingKeySuffixForLinePriorityForIrancell) ?? defaultPriority;
            var rightelPriority = settings.GetInt(settingKeyPrefix + SettingKeySuffixForLinePriorityForRightel) ?? defaultPriority;

            if (string.IsNullOrWhiteSpace(number))
                number = string.Empty;

            number = number.Trim();
            if (!string.IsNullOrEmpty(number) && !number.StartsWith(prefix))
                throw new InvalidOperationException("The configured " + prefix + " number is not valid.");

            return new FarapayamakSenderLine
            {
                LineNumber = number,
                LineDefaultPriority = defaultPriority.GetValueOrDefault(-1),
                LinePriorityForIrancell = irancellPriority.GetValueOrDefault(-1),
                LinePriorityForRightel = rightelPriority.GetValueOrDefault(-1),
                Status = string.IsNullOrWhiteSpace(number) ? FarapayamakSenderLineStatus.NotConfigured : FarapayamakSenderLineStatus.Unknown,
                IsFlashCapable = isFlashCapable
            };
        }

        #endregion

        public static void RegisterConfigurationKeys()
        {
            foreach (var prefix in new [] { SettingKeyFor1KLine, SettingKeyFor2KLine, SettingKeyFor3KLine, SettingKeyFor5KLine, SettingKeyFor021Line })
            {
                ApplicationSettingKeys.RegisterKey(prefix + SettingKeySuffixForLineNumber);
                ApplicationSettingKeys.RegisterKey(prefix + SettingKeySuffixForLineDefaultPriority);
                ApplicationSettingKeys.RegisterKey(prefix + SettingKeySuffixForLinePriorityForIrancell);
                ApplicationSettingKeys.RegisterKey(prefix + SettingKeySuffixForLinePriorityForRightel);
            }
        }
    }
}