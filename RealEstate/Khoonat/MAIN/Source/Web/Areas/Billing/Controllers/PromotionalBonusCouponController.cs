using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [Authorize]
    public class PromotionalBonusCouponController : CustomControllerBase
    {
        [HttpGet]
        public ActionResult ViewClaimedCoupons()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewRewardedBonuses()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ClaimCoupon()
        {
            return View();
        }
    }
}