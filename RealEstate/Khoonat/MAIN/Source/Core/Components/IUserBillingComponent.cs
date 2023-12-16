using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Components
{
    [Contract]
    public interface IUserBillingComponent
    {
        UserBillingApplyResult PartiallyApply<T>(T entity, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity;
        UserBillingApplyResult RecalculatePartiallyApplied<T>(T entity, bool keepPartial = true) where T : class, IBillingSourceEntity;

        UserBillingApplyResult Apply<T>(T entity, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity;
        List<UserBillingApplyResult> ApplyAll<T>(IEnumerable<T> entities, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity;

        UserBillingApplyResult PartiallyApplyOrRecalculate<T>(T entity) where T : class, IBillingSourceEntity;
        List<UserBillingApplyResult> PartiallyApplyOrRecalculate<T>(IEnumerable<T> entities) where T : class, IBillingSourceEntity;
        UserBillingApplyResult ApplyOrFinalizePartiallyApplied<T>(T entity) where T : class, IBillingSourceEntity;
        List<UserBillingApplyResult> ApplyOrFinalizePartiallyApplied<T>(IEnumerable<T> entities) where T : class, IBillingSourceEntity;

        UserBillingApplyResult Reverse<T>(T entity) where T : class, IBillingSourceEntity;
        void Cancel<T>(T entity) where T : class, IBillingSourceEntity;

	    void RecalculateUserBalanceHistory(long userId, DateTime? startTime = null);
		UserBillingBalance CalculateBalance(long userId, DateTime? asOf = null, bool includeAsOfMoment = true);
    }
}