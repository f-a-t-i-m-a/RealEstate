using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using log4net;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Notification.Email
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class SmtpEmailTransmitter : IEmailTransmitter, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SmtpEmailTransmitter));

        private const string AppSettingKeyForServerHost = "SmtpEmailTransmitter.Server.Host";
        private const string AppSettingKeyForServerPort = "SmtpEmailTransmitter.Server.Port";
        private const string AppSettingKeyForLoginName = "SmtpEmailTransmitter.Credentials.LoginName";
        private const string AppSettingKeyForPassword = "SmtpEmailTransmitter.Credentials.Password";
        private const string AppSettingKeyForUseDefaultCredentials = "SmtpEmailTransmitter.Credentials.UseDefault";
        private const string AppSettingKeyForUseSsl = "SmtpEmailTransmitter.UseSSL";
        private const string AppSettingKeyForFromAddress = "SmtpEmailTransmitter.FromAddress";

        static SmtpEmailTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForServerHost);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForServerPort);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForLoginName);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForPassword);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForUseDefaultCredentials);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForUseSsl);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForFromAddress);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #region Configuration properties

        private string SmtpServerHost => ApplicationSettings[AppSettingKeyForServerHost];

        private int SmtpServerPort
        {
            get
            {
                int result;
                return int.TryParse(ApplicationSettings[AppSettingKeyForServerPort], out result) ? result : 25;
            }
        }

        private string SmtpServerLoginName => ApplicationSettings[AppSettingKeyForLoginName];

        private string SmtpServerLoginPassword => ApplicationSettings[AppSettingKeyForPassword];

        private bool UseDefaultCredentials
        {
            get
            {
                bool result;
                return bool.TryParse(ApplicationSettings[AppSettingKeyForUseDefaultCredentials], out result) && result;
            }
        }

        private bool UseSsl
        {
            get
            {
                bool result;
                return bool.TryParse(ApplicationSettings[AppSettingKeyForUseSsl], out result) && result;
            }
        }

        private string FromAddress => ApplicationSettings[AppSettingKeyForFromAddress];

        #endregion

        #region Initialization

        public SmtpEmailTransmitter()
        {
            Log.Debug("Initializing a new instance of SMTP Email Transmitter");
        }

        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
            Log.Debug("Configuration and composition is complete. Configuration summary:");
            Log.DebugFormat("    Server: {0} port {1}", SmtpServerHost, SmtpServerPort);
            Log.DebugFormat("    Login name: {0}", SmtpServerLoginName);
            Log.DebugFormat("    Use default credentials: {0}", UseDefaultCredentials);
            Log.DebugFormat("    Use SSL: {0}", UseSsl);
            Log.DebugFormat("    From Address: {0}", FromAddress);
        }

        #endregion

        public ValidationResult Send(string subject, string body, string[] toAddresses, string[] ccAddresses,
            string[] bccAddresses)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                Log.Error("An error has been occured in sending verification email(s), subject is empty");
                return ValidationResult.Success;
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                Log.Error("An error has been occured in sending verification email(s), body is empty");
                return ValidationResult.Success;
            }
            if (toAddresses == null || !toAddresses.Any())
            {
                Log.Error("An error has been occured in sending verification email(s), destination address(es) is empty");
                return ValidationResult.Success;
            }

            if (ccAddresses == null)
                ccAddresses = new string[0];
            if (bccAddresses == null)
                bccAddresses = new string[0];

            MailMessage msg;
            try
            {
                msg = new MailMessage
                {
                    From = new MailAddress(FromAddress),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                foreach (var address in toAddresses)
                    msg.To.Add(new MailAddress(address));
                foreach (var address in ccAddresses)
                    msg.CC.Add(new MailAddress(address));
                foreach (var address in bccAddresses)
                    msg.Bcc.Add(new MailAddress(address));

                SmtpClient client;

                if (UseDefaultCredentials)
                {
                    client = new SmtpClient(SmtpServerHost, SmtpServerPort)
                    {
                        EnableSsl = UseSsl,
                        UseDefaultCredentials = true
                    };
                }
                else
                {
                    var loginInfo = new NetworkCredential(SmtpServerLoginName, SmtpServerLoginPassword);
                    client = new SmtpClient(SmtpServerHost, SmtpServerPort)
                    {
                        EnableSsl = UseSsl,
                        Credentials = loginInfo
                    };
                }

                Task<bool>.Factory.StartNew(() =>
                {
                    try
                    {
                        ApplicationStaticLogs.EmailTx.InfoFormat(
                            "TX To {0}; CC {1}; BCC {2} - Subject: '{3}' - Length: {4}",
                            toAddresses.Join(), ccAddresses.Join(), bccAddresses.Join(),
                            subject.Truncate(30), body.Length);

                        client.Send(msg);

                        ApplicationStaticLogs.EmailTx.Info("    > OK");
                        return true;
                    }
                    catch (Exception e)
                    {
                        ApplicationStaticLogs.EmailTx.ErrorFormat("    > FAILED with error: {0}", e.Message);
                        Log.Error("Email transmission failed.", e);
                        throw new Exception("Email transmission failed.");
                    }
                });
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.EmailTx.ErrorFormat("Creating email FAILED with error: {0}", e.Message);
            }

            return ValidationResult.Success;
        }
    }
}