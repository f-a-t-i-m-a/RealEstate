using System.ComponentModel.DataAnnotations;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminOutgoingSms
{
    public class AdminOutgoingSmsNewAdvertisementModel
    {
        [Required]
        [StringLength(201, MinimumLength = 10)]
        public string MessageText { get; set; } 

        [Required]
        public string TargetNumbers { get; set; }
    }
}