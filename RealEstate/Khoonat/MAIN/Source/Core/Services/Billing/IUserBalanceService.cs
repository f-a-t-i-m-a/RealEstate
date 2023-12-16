using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Services.Billing
{
    [Contract]
    public interface IUserBalanceService
    {
        void CreateAdministrativeChange(UserBalanceAdministrativeChange change);
        void UpdateAdministrativeChange(UserBalanceAdministrativeChange change);
        UserBillingApplyResult CalculateEffectOfAdministrativeChange(long changeId);
        UserBillingApplyResult ReviewAdministrativeChange(long changeId, bool confirmed);
        UserBillingApplyResult ReverseAdministrativeChange(long changeId);

        void OnUserCostProcessed(UserBillingBalance userBalance, long? sourceEntityID, NotificationSourceEntityType? notificationSourceEntityType);
        void OnUserBalanceIncreased(long userId);

    }
}