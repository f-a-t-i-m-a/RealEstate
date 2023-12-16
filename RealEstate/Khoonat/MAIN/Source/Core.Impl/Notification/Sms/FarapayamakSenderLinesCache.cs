using System;
using System.Linq;
using System.Text.RegularExpressions;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Impl.ExternalServices.Farapayamak.AlertsWebService;
using JahanJooy.RealEstate.Util.Log4Net;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Sms
{
    internal class FarapayamakSenderLinesCache
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (FarapayamakSenderLinesCache));

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

        private DateTime _expirationTime;

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

        public void Initialize(GetAlertsResponseBody alertsResponseBody)
        {
            // Default expiration time is set to 15 minutes, unless we don't have any health information
            _expirationTime = DateTime.Now + TimeSpan.FromMinutes(alertsResponseBody == null ? 5 : 15);

            // Extract health information from alerts response
            SetLineHealth(alertsResponseBody);

            // Decrease line statuses to blocked for lines that have time limitations
            SetBlockedNumbers();

            RealEstateStaticLogs.Farapayamak.InfoFormat("Lines health re-initized. 1000: {0}; 2000: {1}; 3000: {2}; 5000: {3}, 021: {4}",
                _1KNumber.Status, _2KNumber.Status, _3KNumber.Status, _5KNumber.Status, _021Number.Status);
        }

        #endregion

        #region Query API

        public bool IsExpired
        {
            get { return _expirationTime < DateTime.Now; }
        }

        public bool Has1KLine
        {
            get { return _1KNumber.Status != FarapayamakSenderLineStatus.NotConfigured; }
        }

        public bool Has2KLine
        {
            get { return _2KNumber.Status != FarapayamakSenderLineStatus.NotConfigured; }
        }

        public bool Has3KLine
        {
            get { return _3KNumber.Status != FarapayamakSenderLineStatus.NotConfigured; }
        }

        public bool Has5KLine
        {
            get { return _5KNumber.Status != FarapayamakSenderLineStatus.NotConfigured; }
        }

        public bool Has021Line
        {
            get { return _021Number.Status != FarapayamakSenderLineStatus.NotConfigured; }
        }

        public FarapayamakSenderLine Get1KLine
        {
            get { return _1KNumber; }
        }

        public FarapayamakSenderLine Get2KLine
        {
            get { return _2KNumber; }
        }

        public FarapayamakSenderLine Get3KLine
        {
            get { return _3KNumber; }
        }

        public FarapayamakSenderLine Get5KLine
        {
            get { return _5KNumber; }
        }

        public FarapayamakSenderLine Get021Line
        {
            get { return _021Number; }
        }

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

        private void SetLineHealth(GetAlertsResponseBody alertsResponseBody)
        {
            // Default to healthy for all lines.
            if (Has1KLine) _1KNumber.Status = FarapayamakSenderLineStatus.Healthy;
            if (Has2KLine) _2KNumber.Status = FarapayamakSenderLineStatus.Healthy;
            if (Has3KLine) _3KNumber.Status = FarapayamakSenderLineStatus.Healthy;
            if (Has5KLine) _5KNumber.Status = FarapayamakSenderLineStatus.Healthy;
            if (Has021Line) _021Number.Status = FarapayamakSenderLineStatus.Healthy;

            if (alertsResponseBody == null)
            {
                // There was a problem calling the alerts web service, so leave all of the configured lines as unknown.

                Log.Warn("Empty alerts, initializing SMS lines without any health information.");

                DecreaseLineStatus(_1KNumber, FarapayamakSenderLineStatus.Unknown);
                DecreaseLineStatus(_2KNumber, FarapayamakSenderLineStatus.Unknown);
                DecreaseLineStatus(_3KNumber, FarapayamakSenderLineStatus.Unknown);
                DecreaseLineStatus(_5KNumber, FarapayamakSenderLineStatus.Unknown);
                DecreaseLineStatus(_021Number, FarapayamakSenderLineStatus.Unknown);
                return;
            }

            // If there are no alerts, there is no need to decrease health status on any lines.
            if (alertsResponseBody.GetAlertsResult == null)
            {
                Log.Debug("No alerts have been returned from Farapayamak. All lines are considered as healthy.");
                return;
            }

            foreach (var alert in alertsResponseBody.GetAlertsResult)
            {
                Log.DebugFormat("Farapayama alert - Location: {0}, Type: {1}, Title: {2}, Text: {3}",
                    alert.AlertLocation, alert.AlertType, alert.AlertTitle, alert.AlertText);

                // TODO: Set line status based on the alert.
            }
        }

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

        private void DecreaseLineStatus(FarapayamakSenderLine line, FarapayamakSenderLineStatus status)
        {
            if ((byte)(line.Status) < (byte)status)
                return;

            line.Status = status;
        }

        private void SetBlockedNumbers()
        {
            var now = DateTime.Now;

            var nineAm = now.Date.AddHours(9);
            var ninePm = now.Date.AddHours(21);
            var tenPm = now.Date.AddHours(22);

            // 5000 numbers are blocked from 10PM to 9AM
            if (now < nineAm || now > tenPm)
                DecreaseLineStatus(_5KNumber, FarapayamakSenderLineStatus.Blocked);

            // 2000 numbers are blocked from 9PM to 9AM
            if (now < nineAm || now > ninePm)
                DecreaseLineStatus(_2KNumber, FarapayamakSenderLineStatus.Blocked);

            // If we are close to the border lines (9AM, 9PM and 10PM) and the expiration
            // time passes the border lines, decrease the validity period so that the
            // cache gets immediately after the border line.

            if (now < nineAm && _expirationTime > nineAm)
                _expirationTime = nineAm;

            if (now < ninePm && _expirationTime > ninePm)
                _expirationTime = ninePm;

            if (now < tenPm && _expirationTime > tenPm)
                _expirationTime = tenPm;
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