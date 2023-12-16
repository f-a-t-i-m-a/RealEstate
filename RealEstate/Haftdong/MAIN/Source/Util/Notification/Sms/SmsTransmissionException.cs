using System;

namespace JahanJooy.RealEstateAgency.Util.Notification.Sms
{
    public class SmsTransmissionException: ApplicationException
    {
        public string Text { get; set; }
        public string Recipient { get; set; }

        public SmsTransmissionException(string message, string text, string recipient)
            : base(message)
        {
            Text = text;
            Recipient = recipient;
        }

        public SmsTransmissionException(string message, Exception innerException, string text, string recipient)
            : base(message, innerException)
        {
            Text = text;
            Recipient = recipient;
        }
    }
}
