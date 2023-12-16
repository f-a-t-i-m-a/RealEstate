using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Notification
{
	[Contract]
	public interface ISmsTransmitter
	{
	    void SendBatch(List<OutgoingSmsMessage> messages);
        void CheckDelivery(List<OutgoingSmsMessage> messages);
        TimeSpan? GetRetryDelay(OutgoingSmsMessage message);
	}
}