using System;
using System.Collections.Generic;

namespace JahanJooy.RealEstate.Core.Impl.Notification.Email
{
	public class EmailTransmissionException : ApplicationException
	{
		public string Subject { get; set; }
		public string Body { get; set; }
		public string ToAddresses { get; set; }
		public string CcAddresses { get; set; }
		public string BccAddresses { get; set; }

		public EmailTransmissionException(Exception innerException, string subject, string body, IEnumerable<string> toAddresses, IEnumerable<string> ccAddresses, IEnumerable<string> bccAddresses)
			: base("Exception while transmitting email message", innerException)
		{
			Subject = subject;
			Body = body;
			ToAddresses = toAddresses != null ? string.Join("; ", toAddresses) : string.Empty;
			CcAddresses = ccAddresses != null ? string.Join("; ", ccAddresses) : string.Empty;
			BccAddresses = bccAddresses != null ? string.Join("; ", bccAddresses) : string.Empty;
		}
	}
}