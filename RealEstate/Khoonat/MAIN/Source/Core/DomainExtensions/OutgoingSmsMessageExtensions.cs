using System;
using System.Collections.Generic;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.DomainExtensions
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