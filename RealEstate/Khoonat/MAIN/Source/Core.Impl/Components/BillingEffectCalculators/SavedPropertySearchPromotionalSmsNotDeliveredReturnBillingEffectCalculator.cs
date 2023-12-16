using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Impl.Components.BillingEffectCalculators
{
    [Component("SavedPropertySearchPromotionalSmsNotDeliveredReturn")]
    public class SavedPropertySearchPromotionalSmsNotDeliveredReturnBillingEffectCalculator : IUserBillingEffectCalculatorComponent
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public CalculatedBillingEntityEffect CalculateBillingEntityEffect(IBillingSourceEntity entity, UserBillingBalance currentBalance, Tarrif tarrif)
        {
            var message = entity as SavedPropertySearchPromotionalSmsNotDeliveredReturn;
            if (message == null)
                throw new ArgumentException("Entity is either null, or of an incorrect type.");

            var originalMessage = message.PromotionalSms;
            if (originalMessage == null)
                originalMessage = DbManager.Db.SavedPropertySearchPromotionalSmses.SingleOrDefault(m => m.ID == message.PromotionalSmsID);

            if (originalMessage == null)
                throw new InvalidOperationException("Invalid PromotionalSmsID in entity properties");

            if (originalMessage.NumberOfSmsSegments < 1)
                throw new InvalidOperationException("Invalid 'number of segments' in the entity");

            var singleSmsCost = tarrif.PromotionalSmsBase + (originalMessage.NumberOfSmsSegments - 1)*tarrif.PromotionalSmsAdditionalPart;
            var totalReturn = singleSmsCost /* TODO: *numberOfNotDeliveredResults*/;

            throw new NotImplementedException("The source entity is not complete yet.");
            return new CalculatedBillingEntityEffect {CashDelta = 0m, BonusDelta = totalReturn};
        }
    }
}