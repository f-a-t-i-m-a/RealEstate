using System.ComponentModel.DataAnnotations;
using System.Linq;
using JahanJooy.Common.Util.General;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.PromotionalBonusAdmin
{
    public class PromotionalBonusAdminBonusModel
    {
        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        public decimal? BonusAmount { get; set; }

        [Required]
        public string TargetUserIDsCsv
        {
            get { return CsvUtils.ToCsvString(TargetUserIDs); }
            set { TargetUserIDs = CsvUtils.ParseInt64Enumerable(value).ToArray(); }
        }

        public long[] TargetUserIDs { get; set; }
        public bool NotifyUser { get; set; }
    }
}