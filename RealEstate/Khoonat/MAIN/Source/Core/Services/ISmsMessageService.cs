using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
    public interface ISmsMessageService
	{
        void EnqueueOutgoingMessage(string messageText, string targetNumber, NotificationReason reason, NotificationSourceEntityType sourceEntityType, long sourceEntityId,
			bool isFlash = false, long? targetUserId = null, DateTime? scheduledDate = null, DateTime? expirationDate = null, bool allowTransmissionOnAnyTimeOfDay = false);

	    void ResetOutgoingMessage(long messageId);
	    void CancelOutgoingMessage(long messageId);

	    bool RunTransmissionBatch();
	    bool RunDeliveryCheckingBatch();
	}
}