using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Services.Billing
{
    [Contract]
    public interface IUserPaymentService
    {
        void CreateWireTransferPayment(UserWireTransferPayment payment);
        UserBillingApplyResult CalculateEffectOfWireTransferPayment(long paymentId);
        UserBillingApplyResult ReviewWireTransferPayment(long paymentId, bool confirmed);
        UserBillingApplyResult ReverseWireTransferPayment(long paymentId);
        UserElectronicPayment CreateElectronicPayment(UserElectronicPayment payment);
        void UpdateElectronicPayment(long id, Action<UserElectronicPayment> updateAction);
        UserBillingApplyResult ApplyElectronicPayment(long id);

    }
}