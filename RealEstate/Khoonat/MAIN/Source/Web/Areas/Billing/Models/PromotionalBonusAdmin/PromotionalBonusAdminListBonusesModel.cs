using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.PromotionalBonusAdmin
{
    public class PromotionalBonusAdminListBonusesModel
    {
        public PagedList<PromotionalBonus> Bonuses { get; set; }
        public string Page { get; set; }

        public PromotionalBonusAdminListBonusesOrder? SortOrder { get; set; }
        public bool SortDescending { get; set; }

        public bool ApplyCreatedByUserIDFilter { get; set; }
        public long? CreatedByUserIDFilter { get; set; }
        public long? TargetUserIDFilter { get; set; }
        public PromotionalBonusReason? ReasonFilter { get; set; }
        public string TextFilter { get; set; }
        public decimal? MinAmountFilter { get; set; }
        public decimal? MaxAmountFilter { get; set; }
    }

    public enum PromotionalBonusAdminListBonusesOrder
    {
        ID,
        CreationTime,
        BonusAmount
    }
}