using System;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    public static class BillingEffectCalculationUtil
    {
        public static CalculatedBillingEntityEffect SplitCost(decimal totalCost, UserBillingBalance currentBalance)
        {
            if (currentBalance == null)
                throw new ArgumentNullException("currentBalance");

            if (totalCost <= 0m)
                throw new ArgumentException("Total Cost should be a positive number. Passed value: " + totalCost);

            // If the user doesn't have any bonus credit, deduct everything from his cash balance (either negative or positive)
            if (currentBalance.BonusBalance <= 0m)
                return new CalculatedBillingEntityEffect { CashDelta = -totalCost, BonusDelta = 0m };

            decimal bonusCost;

            if (currentBalance.CashBalance <= 0m)
            {
                // If there's no cash in user's balance, deduct everything from bonus.
                bonusCost = totalCost;
            }
            else
            {
                // If both cash balance and bonus balance are positive, split the cost relative to their proportion
                bonusCost = currentBalance.BonusBalance/currentBalance.TotalBalance*totalCost;
            }

            // Round the bonus cost (can have fractions because of division)
			bonusCost = Math.Round(bonusCost, BillingContants.MoneyPrecision);

            // Bonus deductibe cannot exceed the bonus balance.
            bonusCost = Math.Min(currentBalance.BonusBalance, bonusCost);

            // Deduct any remaining amount from the cash balance.
            var cashCost = totalCost - bonusCost;

            return new CalculatedBillingEntityEffect { CashDelta = -cashCost, BonusDelta = -bonusCost };
        }
    }
}