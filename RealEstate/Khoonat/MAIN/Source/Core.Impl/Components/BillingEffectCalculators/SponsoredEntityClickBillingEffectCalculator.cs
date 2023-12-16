using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("SponsoredEntityClickBilling")]
    public class SponsoredEntityClickBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var click = entity as SponsoredEntityClickBilling;
            if (click == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (click.TotalAmount < 0)
                throw new InvalidOperationException("Invalid 'Total Amount' in the entity");

            return BillingEffectCalculationUtil.SplitCost(click.TotalAmount, currentBalance);
        }
    }
}