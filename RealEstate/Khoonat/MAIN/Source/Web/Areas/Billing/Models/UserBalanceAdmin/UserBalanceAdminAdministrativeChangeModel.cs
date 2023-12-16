using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.UserBalanceAdmin
{
    [Bind(Exclude = "AdministrativeChange")]
    public class UserBalanceAdminAdministrativeChangeModel
    {
        #region Input properties

        public long? ID { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string AdministrativeNotes { get; set; }

        [Required]
        public decimal? CashDelta { get; set; }

        [Required]
        public decimal? BonusDelta { get; set; }

        [Required]
        public long? TargetUserID { get; set; }

        #endregion

        #region Data output properties

        public UserBalanceAdministrativeChange AdministrativeChange { get; set; }

        #endregion
    }
}