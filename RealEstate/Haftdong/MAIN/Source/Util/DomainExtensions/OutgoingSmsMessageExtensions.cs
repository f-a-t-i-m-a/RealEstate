using System;
using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.Messages;

namespace JahanJooy.RealEstateAgency.Util.DomainExtensions
{
    public static class OutgoingSmsMessageExtensions
    {
        public static void SetState(this OutgoingSmsMessage message, OutgoingSmsMessageState newState)
        {
            message.State = newState;
            message.StateDate = DateTime.Now;
        }

        public static void SetState(this IEnumerable<OutgoingSmsMessage> messages, OutgoingSmsMessageState newState)
        {
            foreach (var message in messages)
            {
                message.SetState(newState);
            }
        }
    }
}
