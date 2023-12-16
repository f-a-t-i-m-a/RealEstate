using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Services.Billing
{
    [Contract]
    public interface IPromotionalBonusService
    {
        IEnumerable<UserBillingApplyResult> CreateUserRequestedBonuses(IEnumerable<PromotionalBonus> bonuses, bool notifyUser);
        UserBillingApplyResult CreateNewAccountSignupBonusIfApplicable(long userId);
        UserBillingApplyResult ReverseBonus(long bonusId);
    }
}