using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.EF;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Services.Resources;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Services.Billing
{
    [Component]
    public class PromotionalBonusService : IPromotionalBonusService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public ITarrifService TarrifService { get; set; }

        [ComponentPlug]
        public ISmsNotificationService SmsNotificationService { get; set; }

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        [ComponentPlug]
        public IUserBalanceService UserBalanceService { get; set; }

        public IEnumerable<UserBillingApplyResult> CreateUserRequestedBonuses(IEnumerable<PromotionalBonus> bonuses,
            bool notifyUser)
        {
            if (bonuses == null)
                throw new ArgumentNullException("bonuses");

            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated || !ServiceContext.Principal.IsOperator)
                throw new InvalidOperationException(
                    "Only operator accounts can create Administrative Changes in user billing");

            var newBonuses = new List<PromotionalBonus>();
            foreach (var bonus in bonuses)
            {
                if (!bonus.TargetUserID.HasValue)
                    throw new InvalidOperationException("TargetUserID is not specified.");

                if (string.IsNullOrWhiteSpace(bonus.Description))
                    throw new InvalidOperationException("Description is either null or empty.");

                newBonuses.Add(new PromotionalBonus
                {
                    CreatedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault(),
                    Description = bonus.Description,
                    Reason = PromotionalBonusReason.OperatorRequest,
                    CreationTime = DateTime.Now,
                    BonusAmount = bonus.BonusAmount,
                    TargetUserID = bonus.TargetUserID,
                    BillingState = BillingSourceEntityState.Pending
                });
            }

            DbManager.Db.PromotionalBonusesDbSet.AddAll(newBonuses);
            DbManager.SaveDefaultDbChanges();

            foreach (var newBonus in newBonuses)
            {
                UserBalanceService.OnUserBalanceIncreased(newBonus.TargetUserID.GetValueOrDefault());

                ActivityLogService.ReportActivity(TargetEntityType.PromotionalBonus, newBonus.ID, ActivityAction.Create,
                    detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: newBonus.ForwardTransactionID);
            }

           
            // Notify by SMS and Email
            if (notifyUser)
            {
                foreach (var bonus in newBonuses)
                {
                    var userId = bonus.TargetUserID;
                    var verifiedContactMethods = DbManager.Db.UserContactMethods
                        .Include(cm => cm.User)
                        .Where(cm => cm.IsVerified && !cm.IsDeleted && cm.UserID == userId)
                        .ToList();

                    var userPhoneContactMethod =
                        verifiedContactMethods.FirstOrDefault(cm => cm.ContactMethodType == ContactMethodType.Phone);
                    var userEmailContactMethod =
                        verifiedContactMethods.FirstOrDefault(cm => cm.ContactMethodType == ContactMethodType.Email);

                    if (userPhoneContactMethod != null)
                        SmsNotificationService.NotifyBonusAwarded(userPhoneContactMethod, bonus);
                    if (userEmailContactMethod != null)
                        EmailNotificationService.NotifyBonusAwarded(userEmailContactMethod, bonus);
                }
            }

            return UserBillingComponent.ApplyAll(newBonuses, false);
        }

        public UserBillingApplyResult CreateNewAccountSignupBonusIfApplicable(long userId)
        {
            // If the user has already received any signup bonuses, don't award any more bonuses.

            if (
                DbManager.Db.PromotionalBonuses.Any(
                    b =>
                        b.TargetUserID == userId && b.Reason == PromotionalBonusReason.NewAccountSignup &&
                        b.BillingState == BillingSourceEntityState.Applied))
                return null;

            // Check to see if the user has any verified phone numbers that are never used before.

            var verifiedPhones =
                DbManager.Db.UserContactMethods.Where(
                    cm =>
                        cm.UserID == userId && cm.IsVerified && !cm.IsDeleted &&
                        cm.ContactMethodType == ContactMethodType.Phone).ToList();
            if (!verifiedPhones.Any())
                return null;

            // Intentionally, not filtering contact methods based on IsDeleted column to prevent delete-and-add misuse from users
            foreach (var verifiedPhone in verifiedPhones)
            {
                var verifiedPhoneText = verifiedPhone.ContactMethodText;

                if (!DbManager.Db.UserContactMethodsDbSet.Any(
                        cm => cm.UserID != userId && cm.ContactMethodText == verifiedPhoneText && cm.IsVerified))
                {
                    return CreateNewAccountSignupBonus(userId);
                }
            }

            // If we got this far, it means all of the user's verified phone numbers are previously used in other accounts.
            // So, don't award the bonus.

            return null;
        }

        public UserBillingApplyResult ReverseBonus(long bonusId)
        {
            var bonus = DbManager.Db.PromotionalBonusesDbSet.SingleOrDefault(b => b.ID == bonusId);
            if (bonus == null)
                throw new ArgumentException("Could not load bonus id " + bonusId);

            var result = UserBillingComponent.Reverse(bonus);

            ActivityLogService.ReportActivity(TargetEntityType.PromotionalBonus, bonus.ID, ActivityAction.Reverse,
                detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: bonus.ReverseTransactionID,
                succeeded: result.Successful);
            return result;
        }

        #region Private helper methods

        public UserBillingApplyResult CreateNewAccountSignupBonus(long userId)
        {
            var amount = TarrifService.GetTarrif(userId).NewAccountVerificationPromotionalBonus;
            var bonus = new PromotionalBonus
            {
                CreatedByUserID = null,
                Description = PromotionalBonusServiceResources.NewAccountSignupBonusDescription,
                Reason = PromotionalBonusReason.NewAccountSignup,
                CreationTime = DateTime.Now,
                BonusAmount = amount,
                TargetUserID = userId,
                BillingState = BillingSourceEntityState.Pending
            };

            DbManager.Db.PromotionalBonusesDbSet.Add(bonus);
            DbManager.SaveDefaultDbChanges();

           // ActivityLogService.ReportActivity(TargetEntityType.Bonus, bonus.ID, ActivityAction.Create);

            var userContactMethod = DbManager.Db.UserContactMethodsDbSet
                       .Include(cm => cm.User)
                       .FirstOrDefault(
                           cm =>
                               cm.IsVerified && !cm.IsDeleted && cm.ContactMethodType == ContactMethodType.Phone &&
                               cm.UserID == userId);
            var userEmailContactMethod = DbManager.Db.UserContactMethodsDbSet
               .Include(cm => cm.User)
               .FirstOrDefault(
                   cm =>
                       cm.IsVerified && !cm.IsDeleted && cm.ContactMethodType == ContactMethodType.Email &&
                       cm.UserID == userId);

            SmsNotificationService.NotifyBonusAwarded(userContactMethod, bonus);
            EmailNotificationService.NotifyBonusAwarded(userEmailContactMethod, bonus);

            return UserBillingComponent.Apply(bonus);
        }

        #endregion
    }
}