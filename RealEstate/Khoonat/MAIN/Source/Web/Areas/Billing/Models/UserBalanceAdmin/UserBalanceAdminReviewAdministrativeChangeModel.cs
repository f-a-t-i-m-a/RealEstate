using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserBalanceAdmin
{
    public class UserBalanceAdminReviewAdministrativeChangeModel
    {
        public UserBalanceAdministrativeChange Change { get; set; }
        public UserBillingApplyResult ApplyResult { get; set; }
    }
}