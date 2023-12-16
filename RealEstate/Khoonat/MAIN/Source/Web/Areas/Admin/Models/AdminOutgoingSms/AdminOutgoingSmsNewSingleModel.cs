using System.ComponentModel.DataAnnotations;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminOutgoingSms
{
    public class AdminOutgoingSmsNewSingleModel
    {
        [Required]
        [StringLength(268, MinimumLength = 10)]
        public string MessageText { get; set; }

        [Required]
        public string TargetNumber { get; set; }

        public bool IsFlash { get; set; }
        public bool AllowTransmissionOnAnyTimeOfDay { get; set; }
    }
}