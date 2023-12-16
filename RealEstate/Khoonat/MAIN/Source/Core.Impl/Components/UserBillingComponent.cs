using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Components
{
    [Component]
    public class UserBillingComponent : IUserBillingComponent
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ITarrifService TarrifService { get; set; }

        [ComponentPlug]
        public IUserBillingEffectCalculatorComponent BillingEffectCalculator { get; set; }

        [ComponentPlug]
        public IUserBillingBalanceCache UserBillingBalanceCache { get; set; }

        public UserBillingApplyResult PartiallyApply<T>(T entity, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity
        {
            var result = ApplyInternal(entity, simulate, true, validateForNegativeBalance);

            if (simulate) 
                return result;

            DbManager.SaveDefaultDbChanges();
            UserBillingBalanceCache.InvalidateItem(result.TargetUserID);
            return result;
        }

        public UserBillingApplyResult RecalculatePartiallyApplied<T>(T entity, bool keepPartial = true) where T : class, IBillingSourceEntity
        {
            var result = RecalculatePartiallyAppliedInternal(entity, keepPartial);

            DbManager.SaveDefaultDbChanges();
            UserBillingBalanceCache.InvalidateItem(result.TargetUserID);

            return result;
        }

        public UserBillingApplyResult Apply<T>(T entity, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity
        {
            var result = ApplyInternal(entity, simulate, false, validateForNegativeBalance);

            if (simulate || !result.Successful) 
                return result;

            DbManager.SaveDefaultDbChanges();
            UserBillingBalanceCache.InvalidateItem(result.TargetUserID);
            return result;
        }

        public List<UserBillingApplyResult> ApplyAll<T>(IEnumerable<T> entities, bool simulate = false, bool validateForNegativeBalance = true) where T : class, IBillingSourceEntity
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var results = entities.Select(e => ApplyInternal(e, simulate, false, validateForNegativeBalance)).ToList();

            if (simulate || !results.Any(r => r.Successful)) 
                return results;

            DbManager.SaveDefaultDbChanges();
            UserBillingBalanceCache.InvalidateItems(results.Where(r => r.Successful).Select(r => r.TargetUserID));
            return results;
        }

        public UserBillingApplyResult PartiallyApplyOrRecalculate<T>(T entity) where T : class, IBillingSourceEntity
        {
            if (entity.BillingState == BillingSourceEntityState.Pending)
                return PartiallyApply(entity, validateForNegativeBalance: false, simulate: false);
            
            if (entity.BillingState == BillingSourceEntityState.PartiallyApplied)
                return RecalculatePartiallyApplied(entity, keepPartial: true);

            throw new InvalidOperationException("Only 'Pending' or 'PartiallyApplied' billing entities are acceptable. Entity's billing state is " + entity.BillingState);
        }

        public List<UserBillingApplyResult> PartiallyApplyOrRecalculate<T>(IEnumerable<T> entities) where T : class, IBillingSourceEntity
        {
            return entities.Select(PartiallyApplyOrRecalculate).ToList();
        }

        public UserBillingApplyResult ApplyOrFinalizePartiallyApplied<T>(T entity) where T : class, IBillingSourceEntity
        {
            if (entity.BillingState == BillingSourceEntityState.Pending)
                return Apply(entity, simulate: false, validateForNegativeBalance: false);

            if (entity.BillingState == BillingSourceEntityState.PartiallyApplied)
                return RecalculatePartiallyApplied(entity, keepPartial: false);

            throw new InvalidOperationException("Only 'Pending' or 'PartiallyApplied' billing entities are acceptable. Entity's billing state is " + entity.BillingState);
        }

        public List<UserBillingApplyResult> ApplyOrFinalizePartiallyApplied<T>(IEnumerable<T> entities) where T : class, IBillingSourceEntity
        {
            return entities.Select(ApplyOrFinalizePartiallyApplied).ToList();
        }

        public UserBillingApplyResult Reverse<T>(T entity) where T : class, IBillingSourceEntity
        {
            var result = ReverseInternal(entity);

            if (result.Successful)
            {
                DbManager.SaveDefaultDbChanges();
                UserBillingBalanceCache.InvalidateItem(result.TargetUserID);
            }

            return result;
        }

        public void Cancel<T>(T entity) where T : class, IBillingSourceEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.BillingState != BillingSourceEntityState.Pending)
                throw new InvalidOperationException("Only 'Pending' billing entities can be cancelled. Entity's billing state is " + entity.BillingState);

            if (entity.ForwardTransaction != null || entity.ForwardTransactionID.HasValue || entity.ReverseTransaction != null || entity.ReverseTransactionID.HasValue)
                throw new InvalidOperationException("The forward / reverse transactions are already specified for the entity. This should not normally happen.");

            entity.BillingState = BillingSourceEntityState.Cancelled;
        }

		public void RecalculateUserBalanceHistory(long userId, DateTime? startTime = null)
		{
			var startingBalance = startTime.HasValue
				? UserBillingBalance.LoadForUserID(DbManager.Db.UserBillingTransactions, userId, startTime, false)
				: UserBillingBalance.Zero(userId);

			var query = DbManager.Db.UserBillingTransactionsDbSet.Where(tx => tx.UserID == userId);
			if (startTime.HasValue)
				query = query.Where(tx => tx.TransactionTime >= startTime.Value);

			var transactionsToRecalc = query.OrderBy(t => t.TransactionTime).ToList();
			RecalculateTransactions(transactionsToRecalc, startingBalance);
		}

		public UserBillingBalance CalculateBalance(long userId, DateTime? asOf = null, bool includeAsOfMoment = true)
        {
            return UserBillingBalance.LoadForUserID(DbManager.Db.UserBillingTransactions, userId, asOf, includeAsOfMoment);
        }

        #region Private helper methods

        private UserBillingApplyResult RecalculatePartiallyAppliedInternal<T>(T entity, bool keepPartial) where T : class, IBillingSourceEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (DbManager.Db.ChangeTracker.Entries<T>().All(e => e.Entity.ID != entity.ID))
                throw new InvalidOperationException("The entity is not tracked by the database, possibly an implementation bug.");

            if (entity.BillingState != BillingSourceEntityState.PartiallyApplied)
                throw new InvalidOperationException("Only 'PartiallyApplied' billing entities can be recalculated. Entity's billing state is " + entity.BillingState);

            if (entity.TargetUser == null && !entity.TargetUserID.HasValue)
                throw new InvalidOperationException("Target user is unknown. Cannot apply a billing transaction with null user reference.");

            if (!entity.ForwardTransactionID.HasValue)
                throw new InvalidOperationException("The forward transaction is not specified for the 'PartiallyApplied' entity. This should not normally happen.");
            if (entity.ReverseTransactionID.HasValue || entity.ReverseTransaction != null)
                throw new InvalidOperationException("The reverse transaction is already specified for the entity. This should not normally happen.");

            UserBillingSourceType sourceType;
            if (!Enum.TryParse(entity.GetType().Name, out sourceType))
                throw new InvalidOperationException("Unsupported source entity type: " + entity.GetType().Name);

            var targetUserId = entity.TargetUserID.HasValue ? entity.TargetUserID.Value : entity.TargetUser.ID;

            if (targetUserId <= 0)
                throw new InvalidOperationException("Target User ID is less than or equal to zero. Possibly a development issue.");
            if (entity.ID <= 0)
                throw new InvalidOperationException("Target Entity ID is less than or equal to zero. Possibly a development issue.");

            var partialTransaction = DbManager.Db.UserBillingTransactionsDbSet.SingleOrDefault(t => t.ID == entity.ForwardTransactionID);
            if (partialTransaction == null ||
                partialTransaction.UserID != targetUserId ||
                partialTransaction.SourceType != sourceType ||
                partialTransaction.SourceID != entity.ID ||
                !partialTransaction.IsPartial)
            {
                throw new InvalidOperationException("The Forward Transaction for an 'InProgress' entity is either null or does not match the entity attributes.");
            }

			var balanceBeforePartial = CalculateBalance(targetUserId, partialTransaction.TransactionTime, false);
            var transactionsAfterPartial = DbManager.Db.UserBillingTransactionsDbSet
                .Where(t => t.UserID == targetUserId && t.TransactionTime > partialTransaction.TransactionTime)
                .OrderBy(t => t.TransactionTime)
                .ToList();

            var result = new UserBillingApplyResult();
            result.TargetUserID = targetUserId;
            result.Successful = true;
            result.FailureReason = null;
            result.SourceEntity = entity;
            result.SourceType = sourceType;
            result.EffectiveTarrif = TarrifService.GetTarrif(targetUserId);
            result.InitialBalanceBeforeRecalculate = balanceBeforePartial;
            result.FinalBalanceBeforeRecalculate = transactionsAfterPartial.LastOrDefault()
                .IfNotNull(t => UserBillingBalance.CreateFromTransaction(t), UserBillingBalance.CreateFromTransaction(partialTransaction));

            RecalculateTransactions(transactionsAfterPartial, balanceBeforePartial);

            result.BalanceBeforeEffect = transactionsAfterPartial.LastOrDefault().IfNotNull(t => UserBillingBalance.CreateFromTransaction(t), balanceBeforePartial);

			var effect = BillingEffectCalculator.CalculateBillingEntityEffect(entity, result.BalanceBeforeEffect, result.EffectiveTarrif);
	        var roundedEffect = new CalculatedBillingEntityEffect
	                        {
								BonusDelta = Math.Round(effect.BonusDelta, BillingContants.MoneyPrecision),
								CashDelta = Math.Round(effect.CashDelta, BillingContants.MoneyPrecision)
	                        };

	        result.Effect = roundedEffect;

            partialTransaction.TransactionTime = DateTime.Now;
            partialTransaction.CashDelta = result.Effect.CashDelta;
            partialTransaction.BonusDelta = result.Effect.BonusDelta;
            RecalculateTransaction(partialTransaction, result.BalanceBeforeEffect);

            if (!keepPartial)
            {
                entity.BillingState = BillingSourceEntityState.Applied;
                partialTransaction.IsPartial = false;
            }

            result.BalanceAfterEffect = UserBillingBalance.CreateFromTransaction(partialTransaction);
            return result;
        }

        private UserBillingApplyResult ApplyInternal<T>(T entity, bool simulate, bool partial, bool validateForNegativeBalance) where T : class, IBillingSourceEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (!simulate)
                if (DbManager.Db.ChangeTracker.Entries<T>().All(e => e.Entity.ID != entity.ID))
                    throw new InvalidOperationException("The entity is not tracked by the database, possibly an implementation bug.");

            if (entity.BillingState != BillingSourceEntityState.Pending)
                throw new InvalidOperationException("Only 'Pending' billing entities can be applied. Entity's billing state is " + entity.BillingState);

            if (entity.TargetUser == null && !entity.TargetUserID.HasValue)
                throw new InvalidOperationException("Target user is unknown. Cannot apply a billing transaction with null user reference.");

            if (entity.ForwardTransaction != null || entity.ForwardTransactionID.HasValue || entity.ReverseTransaction != null || entity.ReverseTransactionID.HasValue)
                throw new InvalidOperationException("The forward / reverse transactions are already specified for the entity. This should not normally happen.");

            UserBillingSourceType sourceType;
            if (!Enum.TryParse(entity.GetType().Name, out sourceType))
                throw new InvalidOperationException("Unsupported source entity type: " + entity.GetType().Name);

            var targetUserId = entity.TargetUserID.HasValue ? entity.TargetUserID.Value : entity.TargetUser.ID;

            if (targetUserId <= 0)
                throw new InvalidOperationException("Target User ID is less than or equal to zero. Possibly a development issue.");
            if (entity.ID <= 0)
                throw new InvalidOperationException("Target Entity ID is less than or equal to zero. Possibly a development issue.");

            var result = new UserBillingApplyResult();
            result.TargetUserID = targetUserId;
            result.Successful = true;
            result.SourceEntity = entity;
            result.SourceType = sourceType;
            result.BalanceBeforeEffect = CalculateBalance(targetUserId);
            result.EffectiveTarrif = TarrifService.GetTarrif(targetUserId);

			var effect = BillingEffectCalculator.CalculateBillingEntityEffect(entity, result.BalanceBeforeEffect, result.EffectiveTarrif);
			var roundedEffect = new CalculatedBillingEntityEffect
			{
				BonusDelta = Math.Round(effect.BonusDelta, BillingContants.MoneyPrecision),
				CashDelta = Math.Round(effect.CashDelta, BillingContants.MoneyPrecision)
			};

			result.Effect = roundedEffect;

            var transaction = BuildTransaction(targetUserId, result.BalanceBeforeEffect, result.Effect, sourceType, entity.ID, false, partial);
            result.BalanceAfterEffect = UserBillingBalance.CreateFromTransaction(transaction);

            //
            // Perform validations

            if (validateForNegativeBalance)
                ValidateNegativeBalanceAfterApplying(result);

            if (!result.Successful)
            {
                // One of the validations have failed. Don't add the transaction.
                return result;
            }

            if (simulate)
            {
                // If simulating, return the result without any changes to the DbSet entities
                return result;
            }

            DbManager.Db.UserBillingTransactionsDbSet.Add(transaction);
            entity.BillingState = partial ? BillingSourceEntityState.PartiallyApplied : BillingSourceEntityState.Applied;
            entity.ForwardTransaction = transaction;

            return result;
        }

        private UserBillingApplyResult ReverseInternal<T>(T entity) where T : class, IBillingSourceEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (DbManager.Db.ChangeTracker.Entries<T>().All(e => e.Entity.ID != entity.ID))
                throw new InvalidOperationException("The entity is not tracked by the database, possibly an implementation bug.");

            if (entity.BillingState != BillingSourceEntityState.Applied)
                throw new InvalidOperationException("Only 'Applied' billing entities can be reversed. Entity's billing state is " + entity.BillingState);

            if (entity.TargetUser == null && !entity.TargetUserID.HasValue)
                throw new InvalidOperationException("Target user is unknown. Cannot apply a billing transaction with null user reference.");

            if (entity.ReverseTransaction != null || entity.ReverseTransactionID.HasValue)
                throw new InvalidOperationException("The reverse transaction is already specified for the entity. This should not normally happen.");

            if (!entity.ForwardTransactionID.HasValue)
                throw new InvalidOperationException("Forward transaction is unknown. Cannot reverse an unknown transaction.");

            UserBillingSourceType sourceType;
            if (!Enum.TryParse(entity.GetType().Name, out sourceType))
                throw new InvalidOperationException("Unsupported source entity type: " + entity.GetType().Name);

            var targetUserId = entity.TargetUserID.HasValue ? entity.TargetUserID.Value : entity.TargetUser.ID;
            var forward = DbManager.Db.UserBillingTransactions.SingleOrDefault(t => t.ID == entity.ForwardTransactionID.Value);

            if (forward == null)
                throw new InvalidOperationException("Forward transaction could not be loaded.");

            if (forward.UserID != targetUserId)
                throw new InvalidOperationException("Forward transaction's user ID does not match the source entity.");

            if (forward.SourceType != sourceType)
                throw new InvalidOperationException("Forward transaction's source type does not match the source entity.");

            if (forward.SourceID != entity.ID)
                throw new InvalidOperationException("Forward transaction's source ID does not match the source entity ID.");

            var result = new UserBillingApplyResult();
            result.TargetUserID = targetUserId;
            result.Successful = true;
            result.SourceEntity = entity;
            result.SourceType = sourceType;
            result.BalanceBeforeEffect = CalculateBalance(targetUserId);
            result.EffectiveTarrif = TarrifService.GetTarrif(targetUserId, forward.TransactionTime);
            result.Effect = new CalculatedBillingEntityEffect { CashDelta = -forward.CashDelta, BonusDelta = -forward.BonusDelta };
            result.InitialBalanceBeforeRecalculate = null;
            result.FinalBalanceBeforeRecalculate = null;

            var transaction = BuildTransaction(targetUserId, result.BalanceBeforeEffect, result.Effect, sourceType, entity.ID, true, false);
            result.BalanceAfterEffect = UserBillingBalance.CreateFromTransaction(transaction);

            DbManager.Db.UserBillingTransactionsDbSet.Add(transaction);
            entity.BillingState = BillingSourceEntityState.Reversed;
            entity.ReverseTransaction = transaction;

            return result;
        }

        private static UserBillingTransaction BuildTransaction(long targetUserId, UserBillingBalance balanceBeforeEffect, CalculatedBillingEntityEffect effect, UserBillingSourceType sourceType, long sourceId, bool isReverse, bool isPartial)
        {
            var result = new UserBillingTransaction
            {
                UserID = targetUserId,
                TransactionTime = DateTime.Now,
                CashDelta = effect.CashDelta,
                BonusDelta = effect.BonusDelta,
                SourceType = sourceType,
                SourceID = sourceId,
                IsReverse = isReverse,
                IsPartial = isPartial,
            };

            RecalculateTransaction(result, balanceBeforeEffect);
            return result;
        }

        private static void RecalculateTransaction(UserBillingTransaction transaction, UserBillingBalance balanceBeforeEffect)
        {
            transaction.CashBalance = balanceBeforeEffect.CashBalance + transaction.CashDelta;
            transaction.BonusBalance = balanceBeforeEffect.BonusBalance + transaction.BonusDelta;
            transaction.CashTurnover = balanceBeforeEffect.CashTurnover + Math.Abs(transaction.CashDelta);
            transaction.BonusTurnover = balanceBeforeEffect.BonusTurnover + Math.Abs(transaction.BonusDelta);
            transaction.HasPartialInHistory = balanceBeforeEffect.IsPartial || balanceBeforeEffect.HasPartialInHistory;
        }

        private static void RecalculateTransactions(IEnumerable<UserBillingTransaction> transactions, UserBillingBalance initialBalance)
        {
            var currentBalance = initialBalance;

            foreach (var transaction in transactions)
            {
                RecalculateTransaction(transaction, currentBalance);
                currentBalance = UserBillingBalance.CreateFromTransaction(transaction);
            }
        }

        private static void ValidateNegativeBalanceAfterApplying(UserBillingApplyResult result)
        {
            // If already failed in some previous validations, ignore this validation.
            if (!result.Successful)
                return;

            // If the resulting balance is non-negative, there's no problem.
            if (result.BalanceAfterEffect.CashBalance >= 0m && result.BalanceAfterEffect.BonusBalance >= 0)
                return;

            // If the cash is positive and bonus is negative (for any reason), and there's no negative change in bonus, it's okay.
            // This means that if the cash balance is negative, user cannot use the bonus balance unless the balance is zeroed.
            if (result.BalanceAfterEffect.CashBalance >= 0m && result.Effect.BonusDelta >= 0m)
                return;

            // If the change is non-negative, consider it valid even if the balance is still negative.
            if (result.Effect.CashDelta >= 0m && result.Effect.BonusDelta >= 0m)
                return;

            result.Successful = false;
            if (result.BalanceAfterEffect.BonusBalance >= 0m && result.BalanceAfterEffect.CashBalance < 0m)
                result.FailureReason = UserBillingApplyFailureReason.CannotUseBonusWhenCashIsNegative;
            else
                result.FailureReason = UserBillingApplyFailureReason.InsufficientBalance;

        }

        #endregion
    }
}