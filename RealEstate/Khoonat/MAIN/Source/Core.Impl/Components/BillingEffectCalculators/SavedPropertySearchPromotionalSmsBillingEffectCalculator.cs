using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("SavedPropertySearchPromotionalSms")]
    public class SavedPropertySearchPromotionalSmsBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var message = entity as SavedPropertySearchPromotionalSms;
            if (message == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (message.NumberOfSmsSegments < 1)
                throw new InvalidOperationException("Invalid 'number of segments' in the entity");

            var singleSmsCost = tarrif.PromotionalSmsBase + (message.NumberOfSmsSegments - 1)*tarrif.PromotionalSmsAdditionalPart;
            var totalCost = singleSmsCost /* TODO: *numberOfMessageTargets*/;

            throw new NotImplementedException("The source entity is not complete yet.");
            return BillingEffectCalculationUtil.SplitCost(totalCost, currentBalance);
        }
    }
}