using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components
{
    [Component]
    public class RootBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        [ComponentPlug]
        public IComposer Composer { get; set; }

        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var component = Composer.GetComponent<IUserBillingEffectCalculatorComponent>(entity.GetType().Name);
            return component.CalculateBillingEntityEffect(entity, currentBalance, tarrif);
        }
    }
}