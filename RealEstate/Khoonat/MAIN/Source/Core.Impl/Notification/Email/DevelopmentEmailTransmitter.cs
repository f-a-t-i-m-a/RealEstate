using System.Collections.Generic;
using System.Net.Mail;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Notification;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Email
{
	[Component]
	[IgnoredOnAssemblyRegistration]
    public class DevelopmentEmailTransmitter : IEmailTransmitter, IEagerComponent
	{
        private const string AppSettingKeyForOutputDirectory = "DevelopmentEmailTransmitter.OutputDirectory";

        static DevelopmentEmailTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForOutputDirectory);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

		public void Send(string subject, string body, IEnumerable<string> toAddresses, IEnumerable<string> ccAddresses, IEnumerable<string> bccAddresses)
		{
			var msg = new MailMessage { From = new MailAddress("account@jj.com"), Subject = subject, Body = body, IsBodyHtml = true };
			foreach (var address in toAddresses)
				msg.To.Add(new MailAddress(address));
			foreach (var address in ccAddresses)
				msg.CC.Add(new MailAddress(address));
			foreach (var address in bccAddresses)
				msg.Bcc.Add(new MailAddress(address));

			var client = new SmtpClient {
				EnableSsl = false,
				UseDefaultCredentials = false,
				DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = ApplicationSettings[AppSettingKeyForOutputDirectory] ?? "C:\\"
			};

            client.Send(msg);
		}
	}
}