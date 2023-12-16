using System;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Util.Log4Net;

namespace JahanJooy.RealEstate.Core.Impl.Services.Billing
{
    [Component]
    public class UserPaymentService : IUserPaymentService, IEagerComponent
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        [ComponentPlug]
        public IUserBalanceService UserBalanceService { get; set; }

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        [ComponentPlug]
        public ISmsNotificationService SmsNotificationService { get; set; }

        public void CreateWireTransferPayment(UserWireTransferPayment payment)
        {
            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated)
                throw new InvalidOperationException("Authentication is required");

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var newPayment = new UserWireTransferPayment
            {
                Amount = payment.Amount,
                SourceBank = payment.SourceBank,
                SourceCardNumberLastDigits = payment.SourceCardNumberLastDigits,
                SourceAccountHolderName = payment.SourceAccountHolderName,
                UserEnteredDate = payment.UserEnteredDate,
                UserEnteredDescription = payment.UserEnteredDescription,
                TargetBank = payment.TargetBank,
                FollowUpNumber = payment.FollowUpNumber,
                CreationTime = DateTime.Now,
                TargetUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault(),
                BillingState = BillingSourceEntityState.Pending
            };

            DbManager.Db.UserWireTransferPaymentsDbSet.Add(newPayment);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.UserWireTransferPayment, newPayment.ID,
                ActivityAction.Create);
        }

        public UserBillingApplyResult CalculateEffectOfWireTransferPayment(long paymentId)
        {
            var payment = DbManager.Db.UserWireTransferPayments.SingleOrDefault(p => p.ID == paymentId);
            return UserBillingComponent.Apply(payment, simulate: true);
        }

        public UserBillingApplyResult ReviewWireTransferPayment(long paymentId, bool confirmed)
        {
            var payment = DbManager.Db.UserWireTransferPaymentsDbSet.SingleOrDefault(p => p.ID == paymentId);

            if (payment == null)
            {
                throw new InvalidOperationException("Could not load UserWireTransferPayment with ID " + paymentId);
            }

            payment.CompletionTime = DateTime.Now;
            payment.ReviewedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault();

            if (confirmed)
            {
                var applyResult = UserBillingComponent.Apply(payment);
                UserBalanceService.OnUserBalanceIncreased(payment.TargetUserID.GetValueOrDefault());

                // Notify by SMS and Email
                var userId = payment.TargetUserID;
                var verifiedContactMethods = DbManager.Db.UserContactMethods
                    .Include(cm => cm.User)
                    .Where(cm => cm.IsVerified && !cm.IsDeleted && cm.UserID == userId)
                    .ToList();

                var userPhoneContactMethod =
                    verifiedContactMethods.FirstOrDefault(cm => cm.ContactMethodType == ContactMethodType.Phone);
                var userEmailContactMethod =
                    verifiedContactMethods.FirstOrDefault(cm => cm.ContactMethodType == ContactMethodType.Email);

                if (userPhoneContactMethod != null)
                    SmsNotificationService.NotifyWireTransferPayment(userPhoneContactMethod, payment);
                if (userEmailContactMethod != null)
                    EmailNotificationService.NotifyWireTransferPayment(userEmailContactMethod, payment);


                ActivityLogService.ReportActivity(TargetEntityType.UserWireTransferPayment, payment.ID,
                    ActivityAction.Approve,
                    detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: payment.ForwardTransactionID);
                return applyResult;
            }


            UserBillingComponent.Cancel(payment);
            ActivityLogService.ReportActivity(TargetEntityType.UserWireTransferPayment, payment.ID,
                ActivityAction.Reject,
                detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: payment.ReverseTransactionID);
            return null;
        }

        public UserBillingApplyResult ReverseWireTransferPayment(long paymentId)
        {
            var payment = DbManager.Db.UserWireTransferPaymentsDbSet.SingleOrDefault(p => p.ID == paymentId);
            if (payment == null)
                throw new ArgumentException("Could not load payment id " + paymentId);
            var reverseWireTransferPayment = UserBillingComponent.Reverse(payment);
            ActivityLogService.ReportActivity(TargetEntityType.UserWireTransferPayment, paymentId,
                ActivityAction.Reverse,
                detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: payment.ReverseTransactionID);
            return reverseWireTransferPayment;
        }

        public UserElectronicPayment CreateElectronicPayment(UserElectronicPayment payment)
        {
            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated)
                throw new InvalidOperationException("Authentication is required");

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var newPayment = new UserElectronicPayment
            {
                Amount = payment.Amount,
                PaymentGatewayProvider = payment.PaymentGatewayProvider,
                CreationTime = DateTime.Now,
                TargetUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault(),
                BillingState = BillingSourceEntityState.Pending
            };

            DbManager.Db.UserElectronicPaymentsDbSet.Add(newPayment);
            DbManager.SaveDefaultDbChanges();
            
            RealEstateStaticLogs.IpgLogger.InfoFormat("Increase Credit action started with Amount : {0}  by Bank: {1} for User : {2}", newPayment.Amount, newPayment.PaymentGatewayProvider, newPayment.TargetUserID);
            ActivityLogService.ReportActivity(TargetEntityType.UserElectronicPayment, newPayment.ID,
                ActivityAction.Create);

            return newPayment;
        }

        public void UpdateElectronicPayment(long id, Action<UserElectronicPayment> updateAction)
        {
            if (updateAction == null)
                throw new ArgumentNullException("updateAction");

            var userElectronicPayment = DbManager.Db.UserElectronicPaymentsDbSet
                .SingleOrDefault(uep => uep.ID == id);

            if (userElectronicPayment == null)
                throw new InvalidOperationException("Could not load userElectronicPayment with ID " + id);

            updateAction(userElectronicPayment);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.UserElectronicPayment, userElectronicPayment.ID,
                ActivityAction.Edit);
        }
        //Applies the changes to the user transaction table. 
        public UserBillingApplyResult ApplyElectronicPayment(long id)
        {
            var userElectronicPayment = DbManager.Db.UserElectronicPaymentsDbSet
                .SingleOrDefault(uep => uep.ID == id);

            if (userElectronicPayment == null)
                throw new InvalidOperationException("Could not load userElectronicPayment with ID " + id);

            var userBillingApplyResult = UserBillingComponent.Apply(userElectronicPayment);
            ActivityLogService.ReportActivity(TargetEntityType.UserElectronicPayment, userElectronicPayment.ID,
                ActivityAction.Edit,
                detailEntity: DetailEntityType.UserBillingTransaction,
                detailEntityID: userElectronicPayment.ForwardTransactionID,
                succeeded: userBillingApplyResult.Successful);
            return userBillingApplyResult;
        }
    }
}