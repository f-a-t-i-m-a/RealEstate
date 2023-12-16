using System.Web.Mvc;

namespace JahanJooy.RealEstateAgency.HaftDong.Controllers
{
    public class AdminAppController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToRoute("AdminApp");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult InitAdminApp()
        {
            return View();
        }
    }
}