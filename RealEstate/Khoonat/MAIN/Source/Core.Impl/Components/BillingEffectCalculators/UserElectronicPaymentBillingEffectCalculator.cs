using System;
using Compositional.Composer;
using JahanJooy.Common.Util.Tarrif;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("UserElectronicPayment")]
    public class UserElectronicPaymentBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        [ComponentPlug]
        public ITarrifService TarrifService { get; set; }

        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var ePayment = entity as UserElectronicPayment;
            if (ePayment == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            var cashAmount = ePayment.Amount;
            if (cashAmount <= 0)
                throw new InvalidOperationException("Zero or negative amount");

            var bonusCoefficient = TarrifCalculationUtil.CalculateSteppingTarrif(cashAmount, tarrif.UserPaymentBonusCoefficients);
            var bonusAmount = bonusCoefficient*cashAmount;

            return new CalculatedBillingEntityEffect {CashDelta = cashAmount, BonusDelta = bonusAmount};
        }
    }
}