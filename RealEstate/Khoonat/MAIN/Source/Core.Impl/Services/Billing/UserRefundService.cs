using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Services.Billing
{
    [Component]
    public class UserRefundService : IUserRefundService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public IUserBillingComponent BillingComponent { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        public UserBillingApplyResult CreateRefundRequest(UserRefundRequest userRefundRequest)
        {
            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated)
                throw new InvalidOperationException("Authentication is required");

            if (userRefundRequest == null)
                throw new ArgumentNullException("userRefundRequest");

            if (userRefundRequest.RequestedMaximumAmount)
            {
                var userId = userRefundRequest.TargetUserID.GetValueOrDefault();
                userRefundRequest.RequestedAmount = BillingComponent.CalculateBalance(userId).CashBalance;
            }

            var newRefundRequest = new UserRefundRequest
            {
                CreationTime = DateTime.Now,
                RequestedMaximumAmount = userRefundRequest.RequestedMaximumAmount,
                RequestedAmount = userRefundRequest.RequestedAmount,
                TargetCardNumber = userRefundRequest.TargetCardNumber,
                TargetShebaNumber = userRefundRequest.TargetShebaNumber,
                TargetBank = userRefundRequest.TargetBank,
                TargetAccountHolderName = userRefundRequest.TargetAccountHolderName,
                UserEnteredReason = userRefundRequest.UserEnteredReason,
                UserEnteredDescription = userRefundRequest.UserEnteredDescription,
                TargetUserID = userRefundRequest.TargetUserID.GetValueOrDefault(),
                BillingState = BillingSourceEntityState.Pending,
            };

            DbManager.Db.UserRefundRequestsDbSet.Add(newRefundRequest);
            DbManager.SaveDefaultDbChanges();


            var userBillingApplyResult = UserBillingComponent.Apply(newRefundRequest);
            ActivityLogService.ReportActivity(TargetEntityType.UserRefundRequest, newRefundRequest.ID, ActivityAction.Create,
                detailEntity: DetailEntityType.UserBillingTransaction,
                detailEntityID: newRefundRequest.ForwardTransactionID,
                succeeded: userBillingApplyResult.Successful);
            return userBillingApplyResult;
        }

        public void ReviewRefundRequest(long refundRequestId, bool confirmed)
        {
            var request = DbManager.Db.UserRefundRequestsDbSet.SingleOrDefault(p => p.ID == refundRequestId);

            if (request == null)
            {
                throw new ArgumentException("Could not load UserRefundRequest with ID " + refundRequestId);
            }

            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated || !ServiceContext.Principal.IsOperator)
                throw new InvalidOperationException("Authentication is required by an operator");

            if (request.BillingState != BillingSourceEntityState.Applied || request.ClearedByUserID.HasValue)
            {
                throw new InvalidOperationException("RefundRequest is not in a valid state to make payment");
            }

            request.ReviewedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault();
            if (confirmed)
            {
                ActivityLogService.ReportActivity(TargetEntityType.UserRefundRequest, request.ID, ActivityAction.Approve);
            }
            else
            {
                request.CompletionTime = DateTime.Now;
                UserBillingComponent.Reverse(request);
                ActivityLogService.ReportActivity(TargetEntityType.UserRefundRequest, request.ID, ActivityAction.Reject,
                    detailEntity: DetailEntityType.UserBillingTransaction, detailEntityID: request.ReverseTransactionID);
            }
        }

        public void PerformPayment(long refundRequestId)
        {
            var request = DbManager.Db.UserRefundRequestsDbSet.SingleOrDefault(p => p.ID == refundRequestId);
            if (request == null)
            {
                throw new ArgumentException("Could not load UserRefundRequest with ID " + refundRequestId);
            }

            if (!ServiceContext.Principal.CoreIdentity.IsAuthenticated || !ServiceContext.Principal.IsOperator)
                throw new InvalidOperationException("Authentication is required by an operator");

            if (request.BillingState != BillingSourceEntityState.Applied ||
                !request.ReviewedByUserID.HasValue ||
                request.ClearedByUserID.HasValue)
            {
                throw new InvalidOperationException("RefundRequest is not in a valid state to make payment");
            }

            request.CompletionTime = DateTime.Now;
            request.ClearedByUserID = ServiceContext.Principal.CoreIdentity.UserId.GetValueOrDefault();

            ActivityLogService.ReportActivity(TargetEntityType.UserRefundRequest, request.ID, ActivityAction.Other,
                ActivityLogDetails.BillingActionDetails.Cleared);
        }
    }
}