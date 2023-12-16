using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Components.Dto
{
    public class UserBillingApplyResult
    {
        public long TargetUserID { get; set; }
        public bool Successful { get; set; }
        public UserBillingApplyFailureReason? FailureReason { get; set; }

        public IBillingSourceEntity SourceEntity { get; set; }
        public UserBillingSourceType SourceType { get; set; }
        public UserBillingBalance BalanceBeforeEffect { get; set; }
        public CalculatedBillingEntityEffect Effect { get; set; }
        public UserBillingBalance BalanceAfterEffect { get; set; }
        public Tarrif EffectiveTarrif { get; set; }

        public UserBillingBalance InitialBalanceBeforeRecalculate { get; set; }
        public UserBillingBalance FinalBalanceBeforeRecalculate { get; set; }
    }

    public enum UserBillingApplyFailureReason : byte
    {
        InsufficientBalance = 1,
        CannotUseBonusWhenCashIsNegative = 2,
    }
}