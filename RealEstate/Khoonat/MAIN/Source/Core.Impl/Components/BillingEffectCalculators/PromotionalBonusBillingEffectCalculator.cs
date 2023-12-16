using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("PromotionalBonus")]
    public class PromotionalBonusBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var bonus = entity as PromotionalBonus;
            if (bonus == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (bonus.BonusAmount <= 0m)
                return CalculatedBillingEntityEffect.Zero;

            return new CalculatedBillingEntityEffect {CashDelta = 0m, BonusDelta = bonus.BonusAmount};
        }
    }
}