using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.RealEstate.Core.Notification
{
	[Contract]
	public interface IEmailTransmitter
	{
		void Send(string subject, string body, IEnumerable<string> toAddresses, IEnumerable<string> ccAddresses, IEnumerable<string> bccAddresses);
	}
}