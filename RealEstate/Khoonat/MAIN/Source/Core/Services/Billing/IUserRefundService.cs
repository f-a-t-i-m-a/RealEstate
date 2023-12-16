using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Services.Billing
{
    [Contract]
    public interface IUserRefundService
    {
        UserBillingApplyResult CreateRefundRequest(UserRefundRequest userRefundRequest);
        void ReviewRefundRequest(long refundRequestId, bool confirmed);
        void PerformPayment(long refundRequestId);
    }
}