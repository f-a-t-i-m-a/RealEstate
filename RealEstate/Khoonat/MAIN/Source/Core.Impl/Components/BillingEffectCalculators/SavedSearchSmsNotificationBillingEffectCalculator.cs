using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("SavedSearchSmsNotificationBilling")]
    public class SavedSearchSmsNotificationBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var notification = entity as SavedSearchSmsNotificationBilling;
            if (notification == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (notification.NumberOfNotificationParts < 1)
                throw new InvalidOperationException("Invalid 'number of segments' in the entity");

            var totalCost = notification.NumberOfNotificationParts * tarrif.SavedPropertySearchSmsNotificationBase;
            return BillingEffectCalculationUtil.SplitCost(totalCost, currentBalance);
        }
    }
}
