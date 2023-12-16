using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("PromotionalBonusCoupon")]
    public class PromotionalBonusCouponBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var coupon = entity as PromotionalBonusCoupon;
            if (coupon == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            if (coupon.CouponValue <= 0m)
                return CalculatedBillingEntityEffect.Zero;

            return new CalculatedBillingEntityEffect {CashDelta = 0m, BonusDelta = coupon.CouponValue};
        }
    }
}