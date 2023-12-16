using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("UserRefundRequest")]
    public class UserRefundRequestBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var refundRequest = entity as UserRefundRequest;
            if (refundRequest == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (currentBalance == null)
                throw new ArgumentNullException("currentBalance");

            if (currentBalance.CashBalance <= 0m)
                return CalculatedBillingEntityEffect.Zero;

            if (refundRequest.RequestedMaximumAmount)
                return new CalculatedBillingEntityEffect { CashDelta = -currentBalance.CashBalance, BonusDelta = -Math.Max(Math.Min(currentBalance.BonusBalance, currentBalance.CashBalance), 0m) };

            if (refundRequest.RequestedAmount <= 0m)
                return CalculatedBillingEntityEffect.Zero;

            var cashDecrease = Math.Max(Math.Min(refundRequest.RequestedAmount, currentBalance.CashBalance), 0m);
            var bonusDecrease = Math.Max(Math.Min(currentBalance.BonusBalance, cashDecrease), 0m);

            return new CalculatedBillingEntityEffect {CashDelta = -cashDecrease, BonusDelta = -bonusDecrease};
        }
    }
}