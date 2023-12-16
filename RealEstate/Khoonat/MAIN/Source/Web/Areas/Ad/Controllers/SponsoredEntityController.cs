using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
    public class SponsoredEntityController : CustomControllerBase
    {
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
