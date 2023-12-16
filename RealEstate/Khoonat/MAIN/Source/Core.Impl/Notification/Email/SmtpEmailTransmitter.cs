using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Util.Log4Net;
using log4net;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Email
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
        private const string AppSettingKeyForUseSSL = "SmtpEmailTransmitter.UseSSL";
        private const string AppSettingKeyForFromAddress = "SmtpEmailTransmitter.FromAddress";

        static SmtpEmailTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForServerHost);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForServerPort);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForLoginName);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForPassword);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForUseDefaultCredentials);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForUseSSL);
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForFromAddress);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

	    #region Configuration properties

	    private string SmtpServerHost
	    {
	        get
	        {
	            return ApplicationSettings[AppSettingKeyForServerHost];
	        }
	    }

	    private int SmtpServerPort
	    {
	        get
	        {
	            int result;
	            return int.TryParse(ApplicationSettings[AppSettingKeyForServerPort], out result) ? result : 25;
	        }
	    }

	    private string SmtpServerLoginName
	    {
	        get
	        {
                return ApplicationSettings[AppSettingKeyForLoginName];
	        }
	    }

	    private string SmtpServerLoginPassword
	    {
	        get
	        {
                return ApplicationSettings[AppSettingKeyForPassword];
	        }
	    }

	    private bool UseDefaultCredentials
	    {
	        get
	        {
	            bool result;
	            return bool.TryParse(ApplicationSettings[AppSettingKeyForUseDefaultCredentials], out result) && result;
	        }
	    }

	    private bool UseSSL
	    {
	        get
	        {
	            bool result;
                return bool.TryParse(ApplicationSettings[AppSettingKeyForUseSSL], out result) && result;
	        }
	    }

	    private string FromAddress
	    {
	        get
	        {
                return ApplicationSettings[AppSettingKeyForFromAddress];
	        }
	    }

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
			Log.DebugFormat("    Use SSL: {0}", UseSSL);
			Log.DebugFormat("    From Address: {0}", FromAddress);
		}

		#endregion

		public void Send(string subject, string body, IEnumerable<string> toAddresses, IEnumerable<string> ccAddresses, IEnumerable<string> bccAddresses)
		{
			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentNullException("subject");
			if (string.IsNullOrWhiteSpace(body))
				throw new ArgumentNullException("body");
			if (toAddresses == null || !toAddresses.Any())
				throw new ArgumentNullException("toAddresses");

			if (ccAddresses == null)
				ccAddresses = Enumerable.Empty<string>();
			if (bccAddresses == null)
				bccAddresses = Enumerable.Empty<string>();

			var msg = new MailMessage {From = new MailAddress(FromAddress), Subject = subject, Body = body, IsBodyHtml = true};
			foreach (var address in toAddresses)
				msg.To.Add(new MailAddress(address));
			foreach (var address in ccAddresses)
				msg.CC.Add(new MailAddress(address));
			foreach (var address in bccAddresses)
				msg.Bcc.Add(new MailAddress(address));

			SmtpClient client;

			if (UseDefaultCredentials)
			{
				client = new SmtpClient(SmtpServerHost, SmtpServerPort) { EnableSsl = UseSSL, UseDefaultCredentials = true };
			}
			else
			{
				var loginInfo = new NetworkCredential(SmtpServerLoginName, SmtpServerLoginPassword);
				client = new SmtpClient(SmtpServerHost, SmtpServerPort) { EnableSsl = UseSSL, Credentials = loginInfo };
			}

			Task<bool>.Factory.StartNew(() =>
			{
				try
				{
					RealEstateStaticLogs.EmailTx.InfoFormat("TX To {0}; CC {1}; BCC {2} - Subject: '{3}' - Length: {4}",
						toAddresses.Join(), ccAddresses.Join(), bccAddresses.Join(),
						subject.Truncate(30), body.Length);

					client.Send(msg);

					RealEstateStaticLogs.EmailTx.Info("    > OK");
					return true;
				}
				catch (Exception e)
				{
					RealEstateStaticLogs.EmailTx.ErrorFormat("    > FAILED with error: {0}", e.Message);
					Log.Error("Email transmission failed.", e);
					return false;
				}
			});
		}
	}
}