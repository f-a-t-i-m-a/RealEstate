using System;
using System.Net.Mail;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Util.Enums;
using log4net;

namespace JahanJooy.RealEstateAgency.Util.Notification.Email
{
    [Component]
    [IgnoredOnAssemblyRegistration]
    public class DevelopmentEmailTransmitter : IEmailTransmitter, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DevelopmentEmailTransmitter));

        private const string AppSettingKeyForOutputDirectory = "DevelopmentEmailTransmitter.OutputDirectory";

        static DevelopmentEmailTransmitter()
        {
            ApplicationSettingKeys.RegisterKey(AppSettingKeyForOutputDirectory);
        }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        public ValidationResult Send(string subject, string body, string[] toAddresses, string[] ccAddresses,
            string[] bccAddresses)
        {
            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress("account@jj.com"),
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

                var client = new SmtpClient
                {
                    EnableSsl = false,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                    PickupDirectoryLocation = ApplicationSettings[AppSettingKeyForOutputDirectory] ?? "C:\\TestLila"
                };
                client.Send(msg);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in sending verification email(s)", e);
            }

            return ValidationResult.Success;
        }
    }
}