using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("UserBalanceAdministrativeChange")]
    public class UserBalanceAdministrativeChangeBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var change = entity as UserBalanceAdministrativeChange;
            if (change == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            return new CalculatedBillingEntityEffect {CashDelta = change.CashDelta, BonusDelta = change.BonusDelta};
        }
    }
}