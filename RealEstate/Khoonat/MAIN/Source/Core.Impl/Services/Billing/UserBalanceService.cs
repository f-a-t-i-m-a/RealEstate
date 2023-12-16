using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Services.Billing
{
    [Component]
    public class UserBalanceService : IUserBalanceService
    {
	    private const int LowBalanceThreshold = 10000;
		private const int DepletedBalanceThreshold = 1000;

	    [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        public void CreateAdministrativeChange(UserBalanceAdministrativeChange change)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            if (!change.TargetUserID.HasValue)
                throw new InvalidOperationException("TargetUserID is not specified.");

            if (string.IsNullOrWhiteSpace(change.Description))
                throw new InvalidOperationException("Description is either null or empty.");

            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated || !ServiceContext.Principal.IsOperator)
                throw new InvalidOperationException(
                    "Only operator accounts can create Administrative Changes in user billing");

            var newChange = new UserBalanceAdministrativeChange
            {
                CreatedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault(),
                Description = change.Description,
                AdministrativeNotes = change.AdministrativeNotes,
                CreationTime = DateTime.Now,
                CashDelta = change.CashDelta,
                BonusDelta = change.BonusDelta,
                TargetUserID = change.TargetUserID,
                BillingState = BillingSourceEntityState.Pending
            };

            DbManager.Db.UserBalanceAdministrativeChangesDbSet.Add(newChange);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.UserBalanceAdministrativeChange, newChange.ID,
                ActivityAction.Create);
        }

        public void UpdateAdministrativeChange(UserBalanceAdministrativeChange change)
        {
            var dbChange = DbManager.Db.UserBalanceAdministrativeChangesDbSet.SingleOrDefault(c => c.ID == change.ID);
            if (dbChange == null)
                throw new InvalidOperationException("Administrative change ID " + change.ID +
                                                    " not found on the database.");

            if (dbChange.BillingState != BillingSourceEntityState.Pending)
                throw new InvalidOperationException(
                    "Can only edit changes in Pending state. UserBalanceAdministrativeChange ID " + change.ID +
                    " is in " + dbChange.BillingState + " state.");

            dbChange.Description = change.Description;
            dbChange.AdministrativeNotes = change.AdministrativeNotes;
            dbChange.CashDelta = change.CashDelta;
            dbChange.BonusDelta = change.BonusDelta;

            ActivityLogService.ReportActivity(TargetEntityType.UserBalanceAdministrativeChange, change.ID,
                ActivityAction.Edit);
        }

        public UserBillingApplyResult CalculateEffectOfAdministrativeChange(long changeId)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChanges.SingleOrDefault(c => c.ID == changeId);
            return UserBillingComponent.Apply(change, simulate: true, validateForNegativeBalance: false);
        }

        public UserBillingApplyResult ReviewAdministrativeChange(long changeId, bool confirmed)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChangesDbSet.SingleOrDefault(c => c.ID == changeId);

            if (change == null)
            {
                throw new InvalidOperationException("Could not load UserBalanceAdministrativeChange with ID " + changeId);
            }

            change.CompletionTime = DateTime.Now;
            change.ReviewedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault();

            if (confirmed)
            {
                var userBillingApplyResult = UserBillingComponent.Apply(change, simulate: false,
                    validateForNegativeBalance: false);
                ActivityLogService.ReportActivity(TargetEntityType.UserBalanceAdministrativeChange, change.ID,
                    ActivityAction.Approve
                    , detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: change.ForwardTransactionID);
                return userBillingApplyResult;
            }

            UserBillingComponent.Cancel(change);
            ActivityLogService.ReportActivity(TargetEntityType.UserBalanceAdministrativeChange, change.ID,
                ActivityAction.Reject
                , detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: change.ReverseTransactionID);
            return null;
        }

        public UserBillingApplyResult ReverseAdministrativeChange(long changeId)
        {
            var change = DbManager.Db.UserBalanceAdministrativeChangesDbSet.SingleOrDefault(c => c.ID == changeId);
            if (change == null)
                throw new ArgumentException("Could not load change id " + changeId);
            var userBillingApplyResult = UserBillingComponent.Reverse(change);
            ActivityLogService.ReportActivity(TargetEntityType.UserBalanceAdministrativeChange, changeId,
                ActivityAction.Reverse,
                detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: change.ReverseTransactionID,
                succeeded: userBillingApplyResult.Successful);
            return userBillingApplyResult;
        }

        public void OnUserCostProcessed(UserBillingBalance userBalance, long? sourceEntityID, NotificationSourceEntityType? notificationSourceEntityType)
        {
            NotificationReason notificationReason;

            var userID = userBalance.UserID;
            if (userBalance.CashBalance < 0 || userBalance.TotalBalance < DepletedBalanceThreshold)
            {
                notificationReason = NotificationReason.BalanceDepleted;
            }
            else if (userBalance.TotalBalance <= LowBalanceThreshold)
            {
                notificationReason = NotificationReason.BalanceRunningLow;
            }
            else
            {
                return;
            }

            var sixWeeksAgo = DateTime.Now.AddDays(-6*7);
            var hasNotificationMessages =
                DbManager.Db.NotificationMessages.Any(nm => nm.TargetUserID == userID && nm.Reason == notificationReason && nm.AddressedTime == null && nm.CreationTime > sixWeeksAgo);

            if (!hasNotificationMessages)
            {
                var notificationMessage = new NotificationMessage
                {
                    CreationTime = DateTime.Now,
                    TargetUserID = userID,
                    Reason = notificationReason,
                    Severity = NotificationSeverity.Normal,
                    NextMessageTransmissionDue = DateTime.Now,
					SourceEntityID = sourceEntityID,
					SourceEntityType = notificationSourceEntityType
                };

                DbManager.Db.NotificationMessagesDbSet.Add(notificationMessage);
            }
        }

        public void OnUserBalanceIncreased(long userId)
        {
            MarkNotificationsAsAddressed(userId);
            MarkSponsoredEntitiesForRecalculation(userId);
        }

        #region Private helper methods

	    private void MarkNotificationsAsAddressed(long userId)
        {
            var notificationMessages = DbManager.Db.NotificationMessagesDbSet
                .Where(nm => nm.TargetUserID == userId && nm.AddressedTime == null &&
                             (nm.Reason == NotificationReason.BalanceRunningLow ||
                              nm.Reason == NotificationReason.BalanceDepleted))
                .ToList();

            foreach (var notificationMessage in notificationMessages)
            {
                notificationMessage.AddressedTime = DateTime.Now;
            }
        }

	    private void MarkSponsoredEntitiesForRecalculation(long userId)
	    {
		    var sponsoredEntities = DbManager.Db.SponsoredEntitiesDbSet.Where(s => s.BilledUserID == userId).ToList();
		    foreach (var sponsoredEntity in sponsoredEntities)
		    {
			    sponsoredEntity.NextRecalcDue = DateTime.Now;
		    }
	    }

	    #endregion
    }
}